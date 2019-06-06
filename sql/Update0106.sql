use uFlights
go
create table cntGroup
(grp varchar(50) not null,
 gr_name varchar(255),
 gr_type varchar(255),
 gr_sys int default(0),
 constraint pk_cntGroup primary key
 (grp)
)
go

create index ind_type on cntGroup(gr_type)
go

insert into cntGroup
select CM_TabNPrefix, CM_Name, 'Компания', 1 from  dbo.cntCompanies

go


CREATE PROCEDURE p_cntGroup_DEL
@grp varchar(50)
 AS
 if exists(SELECT grp FROM cntGroup (NOLOCK)
	WHERE 
	grp = @grp and gr_sys = 1)
 begin
	raiserror('Нельзя удалить системную группу', 16,1)
	return
 end	
	
 DELETE FROM cntGroup
 WHERE 
grp = @grp
GO

--drop procedure p_cntGroup_EDIT
CREATE PROCEDURE p_cntGroup_EDIT
@grp varchar(50)
,@gr_name varchar(255)
,@gr_type varchar(255)
--,@gr_sys int
AS

set @gr_type = 'Роль'

if exists(SELECT grp FROM cntGroup (NOLOCK)
	WHERE 
	grp = @grp and gr_sys = 1)
 begin
	raiserror('Нельзя редактировать системную группу', 16,1)
	return
 end	


IF EXISTS(SELECT grp FROM cntGroup (NOLOCK)
 WHERE 
grp = @grp
)
UPDATE cntGroup
SET 
gr_name = @gr_name
,gr_type = @gr_type
--,gr_sys = @gr_sys
 WHERE 
grp = @grp
ELSE
BEGIN
INSERT INTO cntGroup(
grp
,gr_name
,gr_type
--,gr_sys
) VALUES (
@grp
,@gr_name
,@gr_type
--,@gr_sys
)
END
SELECT * from cntGroup (nolock) where grp = @grp
GO


--grp,gr_name,gr_type


create table cntAccess
( 
ac_pk uniqueidentifier not null default(newid()),
grp varchar(50),
Account varchar(50),
constraint pk_cntAccess primary key 
(ac_pk)
)
go

create index ind_grp on cntAccess(grp)

go

create index ind_Account on cntAccess(Account)

go

create index ind_grp_Account on cntAccess(grp, Account)

go


insert into cntAccess (grp, Account)
select CompCode, AD_Name from cntEmployees
GO

--select * from cntGroup
--drop function fn_cnt_SubscribersOfGroups
create function fn_cnt_SubscribersOfGroups (@grp varchar(50))
returns table
as
return
(
select * from
(
select 
	e.ТабN, 
	e.ИмяСотрудника,
	e.ПолнИмяСотрудника,
	e.Организация,
	e.Должность,
	e.Принят,
	e.Уволен,
	e.AD_Name,
	e.AD_Name Account
from cntEmployees e(nolock)	
union all
select 
	'Роль' ТабN, 
	g.gr_name ИмяСотрудника,
	g.gr_name ПолнИмяСотрудника,
	'Роль' Организация,
	'Роль' Должность,
	'' Принят,
	'' Уволен,
	g.grp AD_Name,
	g.grp Account
from cntGroup g(nolock)	
where g.gr_type = 'Роль'
) e
where e.AD_Name in (select Account from cntAccess (nolock) where grp = @grp)
)
go

--drop function f_cnt_freeSubscribersOfGroups
create function f_cnt_freeSubscribersOfGroups (@grp varchar(50))
returns table
as
return
(
select * from 
(
select 
	e.ТабN, 
	e.ИмяСотрудника,
	e.ПолнИмяСотрудника,
	e.Организация,
	e.Должность,
	e.Принят,
	e.Уволен,
	e.AD_Name,
	e.AD_Name Account
from cntEmployees e(nolock)
union all
select 
	'Роль' ТабN, 
	g.gr_name ИмяСотрудника,
	g.gr_name ПолнИмяСотрудника,
	'Роль' Организация,
	'Роль' Должность,
	'' Принят,
	'' Уволен,
	g.grp AD_Name,
	g.grp Account
from cntGroup g(nolock)	
where g.gr_type = 'Роль'
) e	
where e.AD_Name not in (select Account from cntAccess (nolock) where grp = @grp)
)
go




create procedure [dbo].[p_cnt_subsGroups_Subscribers_DEL]
	@LT_PK varchar(50),
    @RT_PK varchar(50)
AS
    delete from cntAccess
    where grp = @LT_PK and Account = @RT_PK

GO

--drop procedure p_AM_subsGroups_Subscribers_EDIT
create procedure [dbo].[p_cnt_subsGroups_Subscribers_EDIT]
	@LT_PK varchar(50),
    @RT_PK varchar(50)
AS
    insert into cntAccess (grp, Account)
    values (@LT_PK, @RT_PK)

GO




CREATE   FUNCTION [dbo].[fn_cntCheckAccess] (@grp VARCHAR(50), @Account VARCHAR(50))
RETURNS INT
AS
BEGIN
DECLARE @RES INT

IF EXISTS (
SELECT TOP 1 grp FROM  cntAccess (NOLOCK) WHERE grp = @grp AND Account = @Account
UNION
SELECT TOP 1 A1.grp FROM  cntAccess A1(NOLOCK) 
INNER JOIN 
(
SELECT O.grp, ISNULL(A2.Account, '')  Account
 FROM
 cntGroup O INNER JOIN  cntAccess A2(NOLOCK) 
 ON A2.grp = O.grp AND A2.Account = @Account
) OO
ON 
(OO.grp = A1.Account)
WHERE A1.grp = @GRP
)
	SET @RES = 1
	   ELSE
	SET @RES = 0 

RETURN @RES
END

GO

insert into cntGroup
values('ALLDogovors', 'Доступ ко всем договорам', 'Уровень доступа', 1)
go

insert into cntGroup
values('Administrators', 'Администраторы', 'Роль', 1)
go

insert into cntGroup
values('cntpersons_Edit', 'Сотрудники', 'Редактирование', 1)
go

insert into cntGroup
values('cntcontractors_Edit', 'Контрагенты', 'Редактирование', 1)
go

insert into cntGroup
values('cntagreements_Edit', 'Договоры', 'Редактирование', 1)
go

insert into cntGroup
values('cntcategories_Edit', 'Категории', 'Редактирование', 1)
go

insert into cntAccess (grp, Account)
select g.grp, a.ACCOUNT  from 
cntGroup g inner join t_AccessReport a
on g.grp = a.grp

go



ALTER function [dbo].[gln_gmc_list] (@Account varchar(50))
returns table
as
return
(
select gln_gmc_name gln from cntCompCode (nolock)
where dbo.fn_cntCheckAccess(CompCode, @Account) > 0
or dbo.fn_cntCheckAccess('ALLDogovors', @Account) > 0
)

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
and gln_gmc_name in (select gln from dbo.gln_gmc_list(@Account))
)




GO



--select * from gln_gmc_list('user')


ALTER PROCEDURE [dbo].[p_cntagreements_DEL_1]
@agr_key int,
@AUDTUSER nvarchar(50) = null,
@AUDTACCOUNT nvarchar(255)=nul
 AS

if (dbo.fn_cntCheckAccess('cntagreements_Edit', @AUDTUSER) = 0)
begin
	raiserror ('Нет прав для удаления записи.', 16, 1)
	return
end


 DELETE FROM cntagreements
 WHERE 
agr_key = @agr_key


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
)
SET @agr_key = @@IDENTITY
END
SELECT * from v_cntagreements where agr_key = @agr_key


GO





ALTER PROCEDURE [dbo].[p_cntcategories_DEL_1]
@category_id int,
@AUDTUSER nvarchar(50) = null,
@AUDTACCOUNT nvarchar(255)=null
 AS
 
 if (dbo.fn_cntCheckAccess('cntcategories_Edit', @AUDTUSER) = 0)
begin
	raiserror ('Нет прав для удаления записи.', 16, 1)
	return
end


 DELETE FROM cntcategories
 WHERE 
category_id = @category_id


GO



ALTER PROCEDURE [dbo].[p_cntcategories_EDIT]
@category_id int
,@category_alias varchar(24)
,@category_full varchar(50)
,@category_comment varchar(50)
,@last_change_user varchar(50) = null
AS

if (dbo.fn_cntCheckAccess('cntcategories_Edit', @last_change_user) = 0)
begin
	raiserror ('Нет прав для изменения записи.', 16, 1)
	return
end

IF EXISTS(SELECT category_id FROM cntcategories (NOLOCK)
 WHERE 
category_id = @category_id
)
UPDATE cntcategories
SET 
category_alias = @category_alias
,category_full = @category_full
,category_comment = @category_comment
 WHERE 
category_id = @category_id
ELSE
BEGIN
INSERT INTO cntcategories(
category_alias
,category_full
,category_comment
) VALUES (
@category_alias
,@category_full
,@category_comment
)
SET @category_id = @@IDENTITY
END
SELECT * from cntcategories where category_id = @category_id


GO




ALTER PROCEDURE [dbo].[p_cntcontractors_DEL_1]
@contractor_id int,
@AUDTUSER nvarchar(50) = null,
@AUDTACCOUNT nvarchar(255)=nul
 AS
 
 
if (dbo.fn_cntCheckAccess('cntcontractors_Edit', @AUDTUSER) = 0)
begin
	raiserror ('Нет прав для удаления записи.', 16, 1)
	return
end


 
 DELETE FROM cntcontractors
 WHERE 
contractor_id = @contractor_id


GO




ALTER PROCEDURE [dbo].[p_cntcontractors_EDIT]
@contractor_id int
,@is_link varchar(50)
,@contractor_is_ok int
,@contractor_alias varchar(255)
,@contractor_full_name varchar(255)
,@contractor_inn varchar(50)
,@contractor_post_address varchar(255)
,@contractor_legal_address varchar(255)
,@contractor_contacn_info varchar(50)
,@contractor_account_info varchar(255)
,@contractor_docs_rec_date datetime
,@contractor_comment varchar(50)
,@buh_contractor_id varchar(255)
,@buh_contractor_alias varchar(255)
,@buh_contractor_full_name varchar(255)
,@contractor_categories varchar(255)
,@buh_contractor_inn varchar(50)
,@buh_contractor_kpp varchar(50)
,@buh_contractor_post_address varchar(255)
,@buh_contractor_legal_address varchar(255)
,@buh_contractor_contacn_info varchar(50)
,@buh_contractor_account_info varchar(255)
,@buh_contractor_comment varchar(50)
,@buh_agreement_info varchar(255)
,@buh_agreement_num varchar(50)
,@buh_agreement_date datetime
,@buh_inn varchar(50)
,@my_comment varchar(50)
,@last_change_user varchar(50)
,@last_change_date_time datetime
AS

if (dbo.fn_cntCheckAccess('cntcontractors_Edit', @last_change_user) = 0)
begin
	raiserror ('Нет прав для изменения записи.', 16, 1)
	return
end

set @contractor_is_ok = isnull(@contractor_is_ok, 0)
set @last_change_date_time = getdate()

IF EXISTS(SELECT contractor_id FROM cntcontractors (NOLOCK)
 WHERE 
contractor_id = @contractor_id
)
UPDATE cntcontractors
SET 
is_link = @is_link
,contractor_is_ok = @contractor_is_ok
,contractor_alias = @contractor_alias
,contractor_full_name = @contractor_full_name
,contractor_inn = @contractor_inn
,contractor_post_address = @contractor_post_address
,contractor_legal_address = @contractor_legal_address
,contractor_contacn_info = @contractor_contacn_info
,contractor_account_info = @contractor_account_info
,contractor_docs_rec_date = @contractor_docs_rec_date
,contractor_comment = @contractor_comment
,buh_contractor_id = @buh_contractor_id
,buh_contractor_alias = @buh_contractor_alias
,buh_contractor_full_name = @buh_contractor_full_name
,contractor_categories = @contractor_categories
,buh_contractor_inn = @buh_contractor_inn
,buh_contractor_kpp = @buh_contractor_kpp
,buh_contractor_post_address = @buh_contractor_post_address
,buh_contractor_legal_address = @buh_contractor_legal_address
,buh_contractor_contacn_info = @buh_contractor_contacn_info
,buh_contractor_account_info = @buh_contractor_account_info
,buh_contractor_comment = @buh_contractor_comment
,buh_agreement_info = @buh_agreement_info
,buh_agreement_num = @buh_agreement_num
,buh_agreement_date = @buh_agreement_date
,buh_inn = @buh_inn
,my_comment = @my_comment
,last_change_user = @last_change_user
,last_change_date_time = @last_change_date_time
 WHERE 
contractor_id = @contractor_id
ELSE
BEGIN
INSERT INTO cntcontractors(
is_link
,contractor_is_ok
,contractor_alias
,contractor_full_name
,contractor_inn
,contractor_post_address
,contractor_legal_address
,contractor_contacn_info
,contractor_account_info
,contractor_docs_rec_date
,contractor_comment
,buh_contractor_id
,buh_contractor_alias
,buh_contractor_full_name
,contractor_categories
,buh_contractor_inn
,buh_contractor_kpp
,buh_contractor_post_address
,buh_contractor_legal_address
,buh_contractor_contacn_info
,buh_contractor_account_info
,buh_contractor_comment
,buh_agreement_info
,buh_agreement_num
,buh_agreement_date
,buh_inn
,my_comment
,last_change_user
,last_change_date_time
) VALUES (
@is_link
,@contractor_is_ok
,@contractor_alias
,@contractor_full_name
,@contractor_inn
,@contractor_post_address
,@contractor_legal_address
,@contractor_contacn_info
,@contractor_account_info
,@contractor_docs_rec_date
,@contractor_comment
,@buh_contractor_id
,@buh_contractor_alias
,@buh_contractor_full_name
,@contractor_categories
,@buh_contractor_inn
,@buh_contractor_kpp
,@buh_contractor_post_address
,@buh_contractor_legal_address
,@buh_contractor_contacn_info
,@buh_contractor_account_info
,@buh_contractor_comment
,@buh_agreement_info
,@buh_agreement_num
,@buh_agreement_date
,@buh_inn
,@my_comment
,@last_change_user
,@last_change_date_time
)
SET @contractor_id = @@IDENTITY
END
SELECT * from cntcontractors where contractor_id = @contractor_id


GO



ALTER PROCEDURE [dbo].[p_cntpersons_DEL_1]
@person_id int,
@AUDTUSER nvarchar(50) = null,
@AUDTACCOUNT nvarchar(255)=null

 AS
 
 if (dbo.fn_cntCheckAccess('cntpersons_Edit', @AUDTUSER) = 0)
begin
	raiserror ('Нет прав для удаления записи.', 16, 1)
	return
end

 
 DELETE FROM cntpersons
 WHERE 
person_id = @person_id


GO




ALTER PROCEDURE [dbo].[p_cntpersons_EDIT]
@person_id int
,@person_alias varchar(50)
,@person_full_name varchar(50)
,@person_adname varchar(255)
,@person_email varchar(50)
,@person_depart varchar(50)
,@person_tab_num varchar(50)
,@last_change_user varchar(50)
,@last_change_date_time datetime
AS

if (dbo.fn_cntCheckAccess('cntpersons_Edit', @last_change_user) = 0)
begin
	raiserror ('Нет прав для изменения записи.', 16, 1)
	return
end

set @last_change_date_time = GETDATE()

IF EXISTS(SELECT person_id FROM cntpersons (NOLOCK)
 WHERE 
person_id = @person_id
)
UPDATE cntpersons
SET 
person_alias = @person_alias
,person_full_name = @person_full_name
,person_adname = @person_adname
,person_email = @person_email
,person_depart = @person_depart
,person_tab_num = @person_tab_num
,last_change_user = @last_change_user
,last_change_date_time = @last_change_date_time
 WHERE 
person_id = @person_id
ELSE
BEGIN
INSERT INTO cntpersons(
person_alias
,person_full_name
,person_adname
,person_email
,person_depart
,person_tab_num
,last_change_user
,last_change_date_time
) VALUES (
@person_alias
,@person_full_name
,@person_adname
,@person_email
,@person_depart
,@person_tab_num
,@last_change_user
,@last_change_date_time
)
SET @person_id = @@IDENTITY
END
SELECT * from cntpersons (NOLOCK) where person_id = @person_id


GO


