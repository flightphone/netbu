use uFlights
go


alter  VIEW v_sysmenuimage 
AS 
 SELECT t_sysmenuimage.idimage,
    'Root/' +  t_sysmenuimage.caption AS caption,
    t_sysmenuimage.caption AS icaption,
    t_sysmenuimage.image_bmp,
    0 AS typeimg
   FROM t_sysmenuimage
UNION ALL
 SELECT i.idimage,
    'Root/' + m.caption AS caption,
    i.caption AS icaption,
    i.image_bmp,
    1 AS typeimg
   FROM t_mnmainmenu m
     JOIN t_sysmenuimage i ON m.caption like i.caption + '%';


GO


--drop function fn_getmenuimageid
CREATE  FUNCTION fn_getmenuimageid(@caption varchar(max))
  RETURNS integer AS
BEGIN
declare @res int
select top 1 @res = idimage from v_sysmenuimage
where Caption = @caption
order by typeimg, icaption desc
return isnull(@res, 0)
END