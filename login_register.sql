USE [login_register]
GO
/****** Object:  Table [dbo].[user_info]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_info](
	[customerId] [int] IDENTITY(1,1) NOT NULL,
	[password] [nvarchar](50) NULL,
	[lastLoginTime] [date] NULL,
	[stat] [int] NULL,
	[lastUpdateDate] [date] NULL,
	[recordStat] [int] NULL,
	[hashType] [nvarchar](50) NULL,
	[email] [nvarchar](100) NOT NULL,
	[block_count] [int] NULL,
 CONSTRAINT [PK_user_info] PRIMARY KEY CLUSTERED 
(
	[customerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[user_info] ON 

INSERT [dbo].[user_info] ([customerId], [password], [lastLoginTime], [stat], [lastUpdateDate], [recordStat], [hashType], [email], [block_count]) VALUES (17, N'tWBOdV6Lj9I=', CAST(N'2019-10-30' AS Date), 2, CAST(N'2019-10-30' AS Date), 1, N'md5', N'havva', 3)
INSERT [dbo].[user_info] ([customerId], [password], [lastLoginTime], [stat], [lastUpdateDate], [recordStat], [hashType], [email], [block_count]) VALUES (18, N'oxODH+zrDW8=', CAST(N'2019-10-30' AS Date), 1, CAST(N'2019-10-30' AS Date), 1, N'md5', N'ahmet', 0)
SET IDENTITY_INSERT [dbo].[user_info] OFF
/****** Object:  StoredProcedure [dbo].[sp_block]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_block]
(
	@Email nvarchar(100) -- Kullanıcıdan gelen email bilgisini temsil eden parametre
)
AS
BEGIN
UPDATE user_info SET  
block_count = block_count+1 WHERE email = @Email  
END  

GO
/****** Object:  StoredProcedure [dbo].[sp_block_control]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_block_control] 
   @BlockControl INT OUTPUT ,
   @Email nvarchar(100)

AS  
BEGIN  
   SELECT @BlockControl = block_count FROM user_info WHERE email=@Email
END


GO
/****** Object:  StoredProcedure [dbo].[sp_change_stat]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_change_stat](
	@Email nvarchar(100)
)
AS
BEGIN
	UPDATE user_info SET  
 stat=2 WHERE email = @Email  
END
GO
/****** Object:  StoredProcedure [dbo].[sp_checkUser]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[sp_checkUser]
(
	@Email nvarchar(100),
	@result int output
)
as
begin
	
	IF EXISTS (select * from user_info where email=@Email)
	set @result=1
	else
	set @result=0
 
	return @result
end


GO
/****** Object:  StoredProcedure [dbo].[sp_InsertData]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[sp_InsertData]  
(  
@Email varchar(100),  
@Password Varchar(50),  
@LastLoginTime Date,
@Stat int,
@LastUpdateDate Date,
@RecordStat int,
@HashType varchar(50),
@BlockCount int
)  
as  
begin  
  
INSERT INTO [dbo].user_info  
           ([email]  
           ,[password]  
           ,[lastLogintime]
		   ,[stat]
		   ,[lastUpdateDate]
		   ,[recordStat]
		   ,[hashType],
		   [block_count])  
     VALUES  
           (  
           @Email,  
           @Password,  
           @LastLoginTime,
		   @Stat,
		   @LastUpdateDate,
		   @RecordStat,
		   @HashType,
		   @BlockCount
          )  
End
GO
/****** Object:  StoredProcedure [dbo].[sp_login]    Script Date: 30.10.2019 22:47:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_login]
(
@Email nvarchar(100), -- Kullanıcıdan gelen CustomerId bilgisini temsil eden parametre
@Password nvarchar(50),  -- Kullanıcıdan gelen kullanıcı şifre bilgisini temsil eden parametre
@Result int output  -- Geriye sonuç değerini döndüren parametre  
)
AS
IF EXISTS (SELECT * FROM user_info WHERE email=@Email AND password=@Password)
-- if exists komutu, parantez içindeki sorgunun değer döndürüp döndürmediğini kontrol eder. Eğer sorgu bir sonuç kümesi döndürüyorsa @Result parametresi 1 değerine eşitleniyor. 
SET @Result=1
ELSE IF EXISTS(SELECT * FROM user_info WHERE email=@Email AND password<>@Password)
SET @Result=2
ELSE IF EXISTS(SELECT * FROM user_info WHERE email!=@Email)
SET @Result=4
ELSE
 
SET @Result=0
 
RETURN @Result
GO
