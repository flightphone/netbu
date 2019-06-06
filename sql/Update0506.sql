use uFlights
go

create view v_cntAccess_ext
as
select grp, Account, max(grp2) grp2, min(aType) aType
from
(
SELECT grp, Account, '' grp2, 0 aType FROM  dbo.cntAccess c(NOLOCK) 
INNER JOIN cntEmployees E(nolock) on c.Account = E.AD_Name
 
union all

SELECT c.grp, c2.Account, c2.grp grp2, 0 aType FROM  cntAccess c(NOLOCK)
inner join cntAccess c2(NOLOCK) on c.Account = c2.grp
INNER JOIN cntEmployees E(nolock) on c2.Account = E.AD_Name

union all

SELECT c.grp, c3.Account, c2.grp + '/' + c3.grp grp2, 0 aType FROM  cntAccess c(NOLOCK)
inner join cntAccess c2(NOLOCK) on c.Account = c2.grp
inner join cntAccess c3(NOLOCK) on c2.Account = c3.grp
INNER JOIN cntEmployees E(nolock) on c3.Account = E.AD_Name

union all

select AD_Name grp, AD_Name Account, '' grp2, 1 aType from cntEmployees (nolock)
) A
group by grp, Account


GO


--drop table cntAccess_ext
create table cntAccess_ext
(grp varchar(50) not null,
 Account varchar(50) not null,
 grp2 varchar(255),
 aType int,
 y int default(0), 
 n int default(1),
 constraint pk_cntAccess_ext primary key
 (grp, Account)
) 
go
 
create index ind_grp2 on  cntAccess_ext(grp)

go

create index ind_Account on  cntAccess_ext(Account)

go

create index ind_aType on  cntAccess_ext(aType)

go

--drop procedure p_cntAccessUpdate
create procedure p_cntAccessUpdate(@aType int)
as
delete from cntAccess_ext 
where aType = @aType or @aType = -1

insert into cntAccess_ext (grp, Account, grp2, aType, y, n)
select grp, Account, grp2, aType, 1 y, 0 n 
from v_cntAccess_ext
where aType = @aType or @aType = -1

go

CREATE INDEX ind_val  on dbo.cntFilterD (VAL)

GO


GO

--drop function [dbo].[fn_cntCheckFilter2]
CREATE function [dbo].[fn_cntCheckFilter2]
 (@FH_PK uniqueidentifier,
  @Account varchar(50)
  )
  RETURNS INTEGER
AS
BEGIN
IF (dbo.fn_cntCheckAccess('ALLDogovors', @Account) = 1)
	RETURN 1

DECLARE @RES INTEGER
SET @RES = 
(
SELECT SUM(A.EX) FROM
(SELECT ANUM, 
AVG(
CASE EXPR
WHEN 'G' THEN ISNULL(A.y, 0)
WHEN 'F'  THEN CASE WHEN dbo.fn_cntCheckFilter2(VAL, @Account) > 0 THEN 1 ELSE 0 END
WHEN '-G' THEN ISNULL(A.n, 1)
WHEN '-F'  THEN CASE WHEN dbo.fn_cntCheckFilter2(VAL, @Account) > 0 THEN 0 ELSE 1 END
END ) EX
FROM cntFilterD D(NOLOCK)
LEFT JOIN cntAccess_ext A(nolock) on D.VAL = A.grp and A.Account = @Account and D.FH_PK = @FH_PK
WHERE D.FH_PK = @FH_PK
GROUP BY ANUM) A
)
SET @RES = ISNULL(@RES, 0)
IF @RES > 0
	SET @RES = 1
RETURN @RES
END



GO

ALTER   FUNCTION [dbo].[fn_cntCheckAccess] (@grp VARCHAR(50), @Account VARCHAR(50))
RETURNS INT
AS
BEGIN
DECLARE @RES INT

IF EXISTS (
SELECT TOP 1 grp FROM  cntAccess_ext (NOLOCK) WHERE grp = @grp AND Account = @Account
)
	SET @RES = 1
	   ELSE
	SET @RES = 0 

RETURN @RES
END


GO


ALTER PROCEDURE [dbo].[p_cntGroup_DEL]
@grp varchar(50)
 AS
 if exists(SELECT grp FROM cntGroup (NOLOCK)
	WHERE 
	grp = @grp and gr_sys = 1)
 begin
	raiserror('Нельзя удалить системную группу', 16,1)
	return
 end	

 DELETE FROM dbo.cntAccess
 WHERE 
 grp = @grp or Account = @grp

 exec p_cntAccessUpdate 0
 
 DELETE FROM cntGroup
 WHERE 
 grp = @grp
GO




ALTER procedure [dbo].[p_cnt_subsGroups_Subscribers_DEL]
	@LT_PK varchar(50),
    @RT_PK varchar(50)
AS
    delete from cntAccess
    where grp = @LT_PK and Account = @RT_PK
	
	exec p_cntAccessUpdate 0

GO



--drop procedure p_AM_subsGroups_Subscribers_EDIT
ALTER procedure [dbo].[p_cnt_subsGroups_Subscribers_EDIT]
	@LT_PK varchar(50),
    @RT_PK varchar(50)
AS
    insert into cntAccess (grp, Account)
    values (@LT_PK, @RT_PK)
	
	exec p_cntAccessUpdate 0

GO


--Процедура открытия скобок
--drop procedure p_cntCloseBracket
create procedure p_cntOpenBracket
	@FH_PK uniqueidentifier
as
declare @ANUM int
declare @FD_PK int
declare @FM_FH uniqueidentifier
declare @AMAX int
declare @tmpTab table (FH_PK uniqueidentifier)

WHILE (EXISTS (select top 1 FD_PK from cntFilterD (nolock) where FH_PK = @FH_PK and EXPR = 'F'))
begin
select top 1  
@ANUM = ANUM, @FD_PK = FD_PK, @FM_FH = VAL
from cntFilterD (nolock) where FH_PK = @FH_PK and EXPR = 'F'
set @AMAX = (select max(ANUM) from cntFilterD (nolock) where FH_PK = @FH_PK)


insert into cntFilterD (FH_PK, ANUM, EXPR, VAL)
select FH_PK, ANUM, EXPR, VAL
FROM
(
select @FH_PK FH_PK, A.NANUM ANUM, F.EXPR, F.VAL from 
cntFilterD F(nolock) inner join
(
select distinct @AMAX + ANUM + 1 NANUM, @ANUM ANUM, ANUM ANUM2 from cntFilterD (nolock) where FH_PK = @FM_FH
) A
on F.FH_PK = @FH_PK and F.FD_PK <> @FD_PK AND F.ANUM = A.ANUM

union all

select @FH_PK FH_PK, A.NANUM ANUM, F.EXPR, F.VAL from 
cntFilterD F(nolock) inner join
(
select distinct @AMAX + ANUM + 1 NANUM, @ANUM ANUM, ANUM ANUM2 from cntFilterD (nolock) where FH_PK = @FM_FH
) A
on F.FH_PK = @FM_FH AND F.ANUM = A.ANUM2
) I
order by ANUM

delete from cntFilterD  
where FH_PK = @FH_PK and ANUM = @ANUM

insert into @tmpTab (FH_PK) values (@FM_FH)

end

delete from cntFilterD where FH_PK in (select FH_PK from @tmpTab)

GO

exec p_cntAccessUpdate -1