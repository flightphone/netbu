use uFlights
go
--drop view v_cntAccess
--select * from v_cntAccess
create view v_cntAccess
as
select
E.ИмяСотрудника,  A.Account UserLogin, A.grp,  A.grp2, G.gr_name
from
(
SELECT Account, grp,  '' grp2 FROM  dbo.cntAccess c(NOLOCK) 
where c.Account in (select AD_Name from cntEmployees (nolock))
 
 
union all

SELECT c2.Account, c.grp,  c2.grp grp2 FROM  cntAccess c(NOLOCK)
inner join cntAccess c2(NOLOCK) on c.Account = c2.grp
where c2.Account in (select AD_Name from cntEmployees (nolock))


union all

SELECT c3.Account, c.grp,  c2.grp + '/' + c3.grp grp2 FROM  cntAccess c(NOLOCK)
inner join cntAccess c2(NOLOCK) on c.Account = c2.grp
inner join cntAccess c3(NOLOCK) on c2.Account = c3.grp
where c3.Account in (select AD_Name from cntEmployees (nolock))
) A inner join
(select
max(E.ИмяСотрудника) ИмяСотрудника, AD_Name
from cntEmployees E(nolock)
group by AD_Name
) E
on A.Account = E.AD_Name
inner join cntGroup G on A.grp = G.grp
go

insert into cntFilterH(FH_PK, FH_Caption, FH_TEXT)
values ('00000000-0000-0000-0000-000000000000', '<Общий>', '')

GO

--DROP PROCEDURE p_cntFilterH_DEL
CREATE PROCEDURE p_cntFilterH_DEL
@FH_PK uniqueidentifier
 AS
 IF @FH_PK = '00000000-0000-0000-0000-000000000000'
 begin
	raiserror('Нельзя удалить общий фильтр', 16, 1)
	return
 end
 DELETE FROM cntFilterD where FH_PK = @FH_PK
 DELETE FROM cntFilterH
 WHERE FH_PK = @FH_PK
GO

--DROP PROCEDURE p_cntFilterH_EDIT
CREATE PROCEDURE p_cntFilterH_EDIT
@FH_PK uniqueidentifier
--,@FH_NN int = null
,@FH_Caption varchar(255)
,@FH_TEXT varchar(max)
AS
IF @FH_PK = '00000000-0000-0000-0000-000000000000'
 begin
	raiserror('Нельзя редактировать общий фильтр', 16, 1)
	return
 end
 
 
 
IF EXISTS(SELECT FH_PK FROM cntFilterH (NOLOCK)
 WHERE 
FH_PK = @FH_PK
)
UPDATE cntFilterH
SET 
--FH_NN = @FH_NN
 FH_Caption = @FH_Caption
,FH_TEXT = @FH_TEXT
 WHERE 
FH_PK = @FH_PK
ELSE
BEGIN
IF (@FH_PK IS NULL )
  SET @FH_PK = NEWID() 
INSERT INTO cntFilterH(
FH_PK, 
--FH_NN
FH_Caption
,FH_TEXT
) VALUES (
@FH_PK, 
--@FH_NN
@FH_Caption
,@FH_TEXT
)
END

declare @sql varchar(max)
set @sql = ''
exec p_fltcompile @FH_PK, @FH_TEXT, @sql output
delete from cntFilterD where FH_PK = @FH_PK
if (@sql <> '')
begin
	EXEC(@sql)
	exec p_cntOpenBracket @FH_PK
end
SELECT * from cntFilterH (nolock) where FH_PK = @FH_PK
GO


ALTER function [dbo].[gln_gmc_list] (@Account varchar(50))
returns table
as
return
(
select gln_gmc_name gln from cntCompCode (nolock)
where dbo.fn_cntCheckAccess(CompCode, @Account) > 0
or dbo.fn_cntCheckAccess('ALLDogovors', @Account) > 0
union all 
select '' gln
)


GO


alter table dbo.cntagreements
add FH_PK uniqueidentifier 

GO

create index ind_FH_PK on cntagreements (FH_PK)

go



ALTER view [dbo].[v_cntagreements]
as
SELECT [agr_key]
      ,[agreement_type]
      ,[agreement_num]
      ,[agreement_date]
      ,a.[contractor_id]
      ,c.contractor_full_name
      ,[gln_role]
      ,[resp_person_id]
      ,p.person_full_name
      ,[agreement_subject]
      ,[expiration_date]
      ,[actual_begin_date]
      ,[actual_finish_date]
      ,a.[categories_id]
      ,t.category_alias
      ,[gln_gmc_name]
      ,[is_agreement_exist]
      ,[agreement_state]
      ,[agr_folder]
      ,[agr_comment]
      ,[changed_by]
      ,[connected_with]
      ,[agreement_is_ok]
      ,a.[last_change_user]
      ,a.[last_change_date_time]
      ,[content_error]
      ,[agreement_link]
      ,[agr_annotation]
      ,[agr_pricing]
      ,[agr_new_company]
      ,[agr_new_type]
      ,[agr_customer_dept]
      ,[agr_customer_office]
      ,[agr_last_month_cost]
      ,[agr_avg_month_cost]
      ,[agr_corrections]
      ,d.ls_status
	  ,d.ls_actuality_date
	  ,a.FH_PK
	  ,f.FH_Caption
  FROM [cntagreements] a(nolock)
  left join cntcontractors c(nolock) on a.contractor_id = c.contractor_id
  left join cntpersons p(nolock) on a.resp_person_id = p.person_id
  left join cntcategories t(nolock) on a.categories_id = t.category_id
  left join cntdadata d on c.contractor_id = d.contractor_id
  left join cntFilterH f(nolock) on a.FH_PK = f.FH_PK


GO




ALTER PROCEDURE [dbo].[p_cntagreements_EDIT]
@agr_key int
,@agreement_type varchar(50)
,@agreement_num varchar(50)
,@agreement_date datetime
,@contractor_id int
,@gln_role varchar(16)
,@resp_person_id int
,@agreement_subject varchar(255)
,@expiration_date datetime
,@actual_begin_date datetime
,@actual_finish_date datetime
,@categories_id int
,@gln_gmc_name varchar(50)
,@is_agreement_exist varchar(50)
,@agreement_state varchar(50)
,@agr_folder varchar(128)
,@agr_comment varchar(500)
,@changed_by int
,@connected_with int
,@agreement_is_ok int
,@last_change_user varchar(50)
,@last_change_date_time datetime
,@content_error varchar(50)
,@agreement_link varchar(255)
,@agr_annotation varchar(500)
,@agr_pricing varchar(500)
,@agr_new_company varchar(50)
,@agr_new_type varchar(128)
,@agr_customer_dept varchar(128)
,@agr_customer_office varchar(128)
,@agr_last_month_cost varchar(50)
,@agr_avg_month_cost varchar(50)
,@agr_corrections varchar(500)
,@FH_PK uniqueidentifier = null
AS

if (dbo.fn_cntCheckAccess('cntagreements_Edit', @last_change_user) = 0)
begin
	raiserror ('Нет прав для изменения записи.', 16, 1)
	return
end

set @agreement_is_ok = 0
set @last_change_date_time = getdate()


IF EXISTS(SELECT agr_key FROM cntagreements (NOLOCK)
 WHERE 
agr_key = @agr_key
)
UPDATE cntagreements
SET 
agreement_type = @agreement_type
,agreement_num = @agreement_num
,agreement_date = @agreement_date
,contractor_id = @contractor_id
,gln_role = @gln_role
,resp_person_id = @resp_person_id
,agreement_subject = @agreement_subject
,expiration_date = @expiration_date
,actual_begin_date = @actual_begin_date
,actual_finish_date = @actual_finish_date
,categories_id = @categories_id
,gln_gmc_name = @gln_gmc_name
,is_agreement_exist = @is_agreement_exist
,agreement_state = @agreement_state
,agr_folder = @agr_folder
,agr_comment = @agr_comment
,changed_by = @changed_by
,connected_with = @connected_with
,agreement_is_ok = @agreement_is_ok
,last_change_user = @last_change_user
,last_change_date_time = @last_change_date_time
,content_error = @content_error
,agreement_link = @agreement_link
,agr_annotation = @agr_annotation
,agr_pricing = @agr_pricing
,agr_new_company = @agr_new_company
,agr_new_type = @agr_new_type
,agr_customer_dept = @agr_customer_dept
,agr_customer_office = @agr_customer_office
,agr_last_month_cost = @agr_last_month_cost
,agr_avg_month_cost = @agr_avg_month_cost
,agr_corrections = @agr_corrections
,FH_PK = @FH_PK
 WHERE 
agr_key = @agr_key
ELSE
BEGIN
INSERT INTO cntagreements(
agreement_type
,agreement_num
,agreement_date
,contractor_id
,gln_role
,resp_person_id
,agreement_subject
,expiration_date
,actual_begin_date
,actual_finish_date
,categories_id
,gln_gmc_name
,is_agreement_exist
,agreement_state
,agr_folder
,agr_comment
,changed_by
,connected_with
,agreement_is_ok
,last_change_user
,last_change_date_time
,content_error
,agreement_link
,agr_annotation
,agr_pricing
,agr_new_company
,agr_new_type
,agr_customer_dept
,agr_customer_office
,agr_last_month_cost
,agr_avg_month_cost
,agr_corrections
,FH_PK
) VALUES (
@agreement_type
,@agreement_num
,@agreement_date
,@contractor_id
,@gln_role
,@resp_person_id
,@agreement_subject
,@expiration_date
,@actual_begin_date
,@actual_finish_date
,@categories_id
,@gln_gmc_name
,@is_agreement_exist
,@agreement_state
,@agr_folder
,@agr_comment
,@changed_by
,@connected_with
,@agreement_is_ok
,@last_change_user
,@last_change_date_time
,@content_error
,@agreement_link
,@agr_annotation
,@agr_pricing
,@agr_new_company
,@agr_new_type
,@agr_customer_dept
,@agr_customer_office
,@agr_last_month_cost
,@agr_avg_month_cost
,@agr_corrections
,@FH_PK
)
SET @agr_key = @@IDENTITY
END
SELECT * from v_cntagreements where agr_key = @agr_key



GO








ALTER function [dbo].[fn_cntagreements](@agreement_state varchar(50), @gln_gmc_name varchar(50), @category_alias varchar(24), @contractor_id int, @Account varchar(50))
returns table
as
return
(
select * from v_cntagreements
where 
(
(agreement_state = @agreement_state or @agreement_state = 'Все')
and (gln_gmc_name = @gln_gmc_name or @gln_gmc_name = 'Все')
and (category_alias = @category_alias or @category_alias = 'Все')
)
and
(
@contractor_id = -1 or contractor_id = @contractor_id
)
and
(
  (
     ISNULL(gln_gmc_name, '') in (select gln from dbo.gln_gmc_list(@Account))
     AND ISNULL(FH_PK, '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000'
  ) or
  (
	 ISNULL(FH_PK, '00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000'
	 AND 
	 dbo.fn_cntCheckFilter2 (FH_PK, @Account) = 1
  )
)  

)


GO



create function [dbo].[fn_cntagreementsSimpl](@contractor_id int, @Account varchar(50))
returns table
as
return
(

select * from v_cntagreements
where 
(
@contractor_id = -1 or contractor_id = @contractor_id
)
and
(
  (
     ISNULL(gln_gmc_name, '') in (select gln from dbo.gln_gmc_list(@Account))
     AND ISNULL(FH_PK, '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000'
  ) or
  (
	 ISNULL(FH_PK, '00000000-0000-0000-0000-000000000000') <> '00000000-0000-0000-0000-000000000000'
	 AND 
	 dbo.fn_cntCheckFilter2 (FH_PK, @Account) = 1
  )
)  

)


GO