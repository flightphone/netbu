USE [uFlights]
GO

/****** Object:  StoredProcedure [dbo].[p_t_sysStatus_EDIT]    Script Date: 06/06/2019 15:15:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[p_t_sysStatus_EDIT]
@idStatus int
,@StatusType varchar(50)
,@Statusname varchar(50)
,@Color int
,@SortOrder int
,@Comment varchar(max)
,@image_bmp varchar(max)
,@Prefix varchar(8)
AS
IF EXISTS(SELECT idStatus FROM t_sysStatus (nolock)
 WHERE 
idStatus = @idStatus
)
UPDATE t_sysStatus
SET 
StatusType = @StatusType
,Statusname = @Statusname
,Color = @Color
,SortOrder = @SortOrder
,Comment = @Comment
,image_bmp = @image_bmp
,Prefix = @Prefix
 WHERE 
idStatus = @idStatus
ELSE
begin
INSERT INTO t_sysStatus(
 StatusType
,Statusname
,Color
,SortOrder
,Comment
,image_bmp
,Prefix

) VALUES (
 @StatusType
,@Statusname
,@Color
,@SortOrder
,@Comment
,@image_bmp
,@Prefix
)

set @idStatus = @@identity
end

select * from t_sysStatus where idStatus = @idStatus


GO




ALTER PROCEDURE [dbo].[p_t_mnMainMenu_EDIT]
 @IDMenu int
,@ORDMenu int
,@Caption varchar(255)
,@Link varchar(255)
,@Link1 varchar(255)
,@Params varchar(max)
,@mnemo varchar(255)
,@App VARCHAR(50)
,@Web nvarchar(250)


AS
IF EXISTS(SELECT IDMenu FROM t_mnMainMenu
 WHERE 
IDMenu = @IDMenu
)
UPDATE t_mnMainMenu
SET 
ORDMenu = @ORDMenu
,Caption = @Caption
,Link = @Link
,App = @App
,Link1 = @Link1
,mnemo = @mnemo
,Params = @Params
,Web = @Web

 WHERE 
IDMenu = @IDMenu
ELSE
BEGIN
SELECT @IDMenu = ISNULL(MAX(IDMenu), 0) + 1 from t_mnMainMenu (nolock)
INSERT INTO t_mnMainMenu(
IDMenu
,ORDMenu
,Caption
,Link
,App
,Link1
,mnemo
,Params
,Web

) VALUES (
@IDMenu
,@ORDMenu
,@Caption
,@Link
,@App
,@Link1
,@mnemo
,@Params
,@Web

)
--SET @IDMenu = @@IDENTITY
END

SELECT * from t_mnMainMenu where IDMenu = @IDMenu




GO



ALTER PROCEDURE [dbo].[p_t_sysFieldMap_EDIT]
@idMap int
,@DecName varchar(255)
,@dstField varchar(50)
,@srcField varchar(50)
,@idDeclare int
,@ClassName varchar(250)
,@GroupDec varchar(50)
,@KeyField int --24.09.2013 MonteNegro
AS
IF EXISTS(SELECT idMap FROM t_sysFieldMap (nolock)
 WHERE 
idMap = @idMap
)
UPDATE t_sysFieldMap
SET 
DecName = @DecName
,dstField = @dstField
,srcField = @srcField
,idDeclare = @idDeclare
,ClassName = @ClassName
,GroupDec = @GroupDec
,KeyField = @KeyField
 WHERE 
idMap = @idMap
ELSE
BEGIN
INSERT INTO t_sysFieldMap(
DecName
,dstField
,srcField
,idDeclare
,ClassName
,GroupDec
,KeyField
) VALUES (
@DecName
,@dstField
,@srcField
,@idDeclare
,@ClassName
,@GroupDec
,@KeyField
)
SET @idMap = @@IDENTITY
END
SELECT * from t_sysFieldMap where idMap = @idMap


GO




ALTER procedure [dbo].[p_t_sysMenuImage_EDIT]
		@idimage int,
		@Caption varchar(500),
		@image_bmp varchar(max),
		@mnemo varchar(255)
as
if exists(select idimage from t_sysMenuImage (nolock) where idimage = @idimage)
	update t_sysMenuImage set 
		Caption = @Caption,
		image_bmp = @image_bmp,
		mnemo = @mnemo
	where idimage = @idimage
else
   begin
   
     SELECT @idimage = ISNULL(MAX(idimage), 0) + 1 from t_sysMenuImage (nolock)
     insert into t_sysMenuImage (idimage, Caption, image_bmp, mnemo)
	 values (@idimage, @Caption, @image_bmp, @mnemo)
   end

select * from t_sysMenuImage where idimage = @idimage


GO



ALTER PROCEDURE [dbo].[p_rpDeclare_Edit]
	@IdDeclare  int,
	@DecName  varchar (255),
	@Descr  varchar (255),
	@DecType  int,
	@DecSQL text,
	@KeyField varchar (50),
	@DispField varchar (50),
	@KeyValue varchar (255),
	@DispValue varchar (255),
	@KeyParamName varchar (50),
	@DispParamName varchar (50),
	@IsBaseName int,
	@Descript text,
	@AddKeys varchar(50),
	@TableName varchar(50),
	@EditProc varchar(50),
	@DelProc varchar(50),
	@image_bmp varchar(max)=null,
	@SaveFieldList varchar(max)
AS
IF EXISTS (SELECT IdDeclare FROM t_rpDeclare (nolock) WHERE IdDeclare = @IdDeclare)
UPDATE t_rpDeclare SET
	DecName = @DecName,
	Descr = @Descr,
	DecType = @DecType,
	DecSQL = @DecSQL,
	KeyField = @KeyField,
	DispField = @DispField,
	KeyValue = @KeyValue,
	DispValue = @DispValue,
	KeyParamName = @KeyParamName,
	DispParamName = @DispParamName,
	IsBaseName = @IsBaseName,
	Descript = @Descript,
	AddKeys = @AddKeys,
	TableName = @TableName,
	EditProc = @EditProc,
	DelProc = @DelProc,
	image_bmp = @image_bmp,
	SaveFieldList = @SaveFieldList

WHERE IdDeclare = @IdDeclare
ELSE
BEGIN
IF (ISNULL(@IdDeclare, 0) =0)
SELECT @IdDeclare = MAX (IdDeclare) + 1 FROM t_rpDeclare

INSERT INTO t_rpDeclare
(
	IdDeclare,
	DecName,
	Descr,
	DecType,
	DecSQL,
	KeyField,
	DispField,
	KeyValue,
	DispValue,
	KeyParamName,
	DispParamName,
	IsBaseName,
	Descript,
	AddKeys,
	TableName,
	EditProc,
	DelProc,
	image_bmp,
	SaveFieldList

)
VALUES
(
	@IdDeclare,
	@DecName,
	@Descr,
	@DecType,
	@DecSQL,
	@KeyField,
	@DispField,
	@KeyValue,
	@DispValue,
	@KeyParamName,
	@DispParamName,
	@IsBaseName,
	@Descript,
	@AddKeys,
	@TableName,
	@EditProc,
	@DelProc,
	@image_bmp,
	@SaveFieldList
)
END

SELECT * from t_rpDeclare where IdDeclare = @IdDeclare

GO
