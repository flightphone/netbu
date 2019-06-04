use uFlights
go
--select * from dbo.uFilterH
--select * from dbo.uFilterD


--drop table [dbo].[cntFilterH]
CREATE TABLE [dbo].[cntFilterH](
    FH_NN int identity(1,1) not null,
	[FH_PK] uniqueidentifier NOT NULL default(newid()),
	FH_Caption varchar(255),
	[FH_TEXT] [varchar](max) NULL,
 CONSTRAINT [PK_cntFilterH] PRIMARY KEY CLUSTERED 
(
	[FH_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

--drop table cntFilterD
create table cntFilterD
(
  FD_PK int identity(1,1) not null,
  FH_PK uniqueidentifier,
  ANUM int,
  EXPR varchar(2),
  VAL varchar(255),
  constraint pk_cntFilterD primary key
  (FD_PK)
) 
GO
  

CREATE NONCLUSTERED INDEX [cntFilterD_ANUM] ON [dbo].[cntFilterD] 
(
	[ANUM] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [cntFilterD_FH] ON [dbo].[cntFilterD] 
(
	[FH_PK] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [cntFilterD_FH_ANUM] ON [dbo].[cntFilterD] 
(
	[FH_PK] ASC,
	[ANUM] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


--DROP function [dbo].[fn_cntCheckFilter]
CREATE function [dbo].[fn_cntCheckFilter]
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
WHEN 'G' THEN CASE WHEN dbo.fn_cntCheckAccess(VAL, @Account) = 1 THEN 1 ELSE 0 END
WHEN 'F'  THEN CASE WHEN dbo.fn_cntCheckFilter(VAL, @Account) > 0 THEN 1 ELSE 0 END
WHEN '-G' THEN CASE WHEN dbo.fn_cntCheckAccess(VAL, @Account) = 1 THEN 0 ELSE 1 END
WHEN '-F'  THEN CASE WHEN dbo.fn_cntCheckFilter(VAL, @Account) > 0 THEN 0 ELSE 1 END
END ) EX
FROM cntFilterD (NOLOCK)
WHERE FH_PK = @FH_PK
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

IF @grp = @Account
	RETURN 1

IF EXISTS (
SELECT TOP 1 grp FROM  cntAccess (NOLOCK) WHERE grp = @grp AND Account = @Account
UNION
SELECT TOP 1 A1.grp FROM  cntAccess A1(NOLOCK) 
INNER JOIN 
(
SELECT O.grp, A2.Account  Account
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



--select * from dbo.uFilterD
/*
select * from dbo.cntEmployees where 
[dbo].[fn_cntCheckFilter] (3, AD_Name) = 1
order by CompCode
*/
GO
--drop function fn_fltFormatFilter
/*
create function fn_fltFormatFilter(@flt varchar(max))
returns varchar(max)
as
begin
set @flt = replace(@flt, ' или ', '|')
set @flt = replace(@flt, ' и ', '&')
set @flt = replace(@flt, ' ', '')
return @flt
end
*/
GO

--drop FUNCTION [dbo].[fn_splitNN]
CREATE   FUNCTION [dbo].[fn_splitNN]
	(@ParString VARCHAR(max), @Sep VARCHAR(1))
RETURNS @ChTable TABLE  ([ID] int, [VAL] VARCHAR(max))
AS
BEGIN
  DECLARE @I INTEGER,
          @N INTEGER,
          @NN INTEGER,
	  @FWORD INTEGER,
	  @T VARCHAR(max)
          
	
  SET @NN = 1
  SET @N=LEN(@ParString) 
  SET @T='' 
  SET @FWORD=0 
  SET @I=0
  
  WHILE @I <= @N 
  BEGIN
     IF  (SUBSTRING(@ParString, @I, 1) = @Sep)
        BEGIN
          IF (@FWORD=1)
            BEGIN
              IF (@T <> '')
	          BEGIN
				INSERT INTO @ChTable ([ID], [VAL]) VALUES (@NN, @T)
				SET @NN = @NN + 1
			  END
              SET @T='' 
              SET @FWORD=0
            END  
        END
     ELSE
        BEGIN
           SET @FWORD=1 
           SET @T = @T + SUBSTRING(@ParString, @I, 1)
        END 
  SET @I = @I + 1 
  END
      IF (@T <> '')
      BEGIN
		INSERT INTO @ChTable ([ID], [VAL]) VALUES (@NN, @T)
		SET @NN = @NN + 1
      END
  RETURN 
END


GO








--drop procedure p_fltcompile 
create procedure p_fltcompile 
	@FH_PK uniqueidentifier,
	@flt varchar(max),
	@sql varchar(max) output
as


set @flt = replace(@flt, ' или ', '|')
set @flt = replace(@flt, ' и ', '&')
set @flt = replace(@flt, ' or ', '|')
set @flt = replace(@flt, ' and ', '&')
set @flt = replace(@flt, ' ', '')
if isnull(@flt, '') = ''
	return

---===============================================Ищем скобки===============================
declare @newflt varchar(max)
declare @i int
declare @n int
declare @ch varchar(1)
declare @outBraket varchar(max)
declare @openbrek int
declare @nopen int
declare @nclose int
declare @fctmp uniqueidentifier
set @i = 1
set @n = len(@flt)
set @openbrek = 0
set @outBraket = ''
set @newflt = ''
while @i <= @n 
begin
	set @ch = substring(@flt, @i, 1)
	if (@ch <> ')'and @ch <> '(')
	begin
		if @openbrek = 0
			set @newflt = @newflt + @ch
		else
			set @outBraket = @outBraket + @ch
	end
	else
	begin
		if (@ch = '(')
		begin
			if @openbrek = 0 
			begin
			--Открываем скобку
			   set @openbrek = 1
			   set @outBraket = ''
			   set @nopen = 1
			   set @nclose = 0
			end   
			else
			begin
				set @outBraket = @outBraket + @ch
				set @nopen = @nopen + 1
			end
		end	
		else
		begin
			if @openbrek = 0 
			begin
			   raiserror ('Лишняя открывающая скобка', 16, 1)
			   return
			end	
			else
			begin
				set @nclose =  @nclose + 1
				if (@nopen = @nclose)
				begin
				  --Ура закрываем
				  set @openbrek = 0
				  set @fctmp = newid()
				  set @newflt = @newflt + '[' + cast(@fctmp as varchar(50)) + ']'
				  --select @outBraket
				  exec p_fltcompile @fctmp, @outBraket, @sql output
				end
				else
				begin
				  set @outBraket = @outBraket + @ch					  
				end
			end	
		end
	end
set @i = @i + 1	
end
if (@openbrek = 1)
	begin
			   raiserror ('Нет закрывающей скобки', 16, 1)
			   return
	end
 	
--Нашли все скобки

set @flt = @newflt
declare @ANUM int
declare @VALS varchar(max)
declare @val varchar(255)
declare @EXPR varchar(2)
declare m cursor local for

select id, val from dbo.fn_splitNN(@flt, '|')
open m
fetch next from m into @ANUM, @VALS
while @@fetch_status = 0
begin
--====================================cicle |====================================

declare c cursor local for
select ID from dbo.fn_split(@VALS, '&')
open c
fetch next from c into @val
while @@fetch_status = 0
begin
--=================================
set @EXPR = ''
if substring(@val, 1, 1) = '!'
	begin
	  set @EXPR = '-'	
	  set @val = substring(@val, 2, len(@val) - 1)	
	end
if 	substring(@val, 1, 1) = '['
	begin
	  set @EXPR = @EXPR + 'F'
	  set @val = substring(@val, 2, len(@val) - 2)	
	end
else
    set @EXPR = @EXPR + 'G'	
set @sql = @sql	+ char(10) + 'insert into cntFilterD(FH_PK, ANUM, EXPR, VAL)' + char(10) + ' values ('
set @sql = @sql	+ '''' + cast(@FH_PK as varchar(50)) + ''', ' + cast(@ANUM as varchar(4)) + ', ''' + @EXPR + ''', ''' + @val + ''')'
fetch next from c into @val
--=================================
end
close c
deallocate c
--====================================cicle |====================================
fetch next from m into @ANUM, @VALS
end
close m
deallocate m

go






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
 grp = @grp
	
 DELETE FROM cntGroup
 WHERE 
 grp = @grp

GO