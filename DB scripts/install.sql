
CREATE TABLE [dbo].[Ingredients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Created] [datetime2](7) NULL,
 CONSTRAINT [PK_Ingredients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IngredientsInMeals]    Script Date: 11/11/2018 9:31:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IngredientsInMeals](
	[Amount] [int] NULL,
	[IngredientId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
 CONSTRAINT [PK_IngredientsInMeals] PRIMARY KEY CLUSTERED 
(
	[IngredientId] ASC,
	[MealId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Meals]    Script Date: 11/11/2018 9:31:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Meals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Description] [nvarchar](max) NULL,
	[Created] [datetime2](7) NULL,
	[Mealtype] [smallint] NULL,
 CONSTRAINT [PK_Meals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 11/11/2018 9:31:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](50) NULL,
	[Created] [datetime2](7) NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TagsOfMeals]    Script Date: 11/11/2018 9:31:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagsOfMeals](
	[MealId] [int] NULL,
	[TagId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeekPlans]    Script Date: 11/11/2018 9:31:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeekPlans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Week] [int] NULL,
	[Created] [datetime2](7) NULL,
	[Sunday] [int] NULL,
	[Monday] [int] NULL,
	[Tuesday] [int] NULL,
	[Wednesday] [int] NULL,
	[Thursday] [int] NULL,
	[Friday] [int] NULL,
	[Saturday] [int] NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_WeekPlans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Ingredients] ADD  CONSTRAINT [DF_Ingredients_Created]  DEFAULT (sysdatetime()) FOR [Created]
GO
ALTER TABLE [dbo].[Meals] ADD  CONSTRAINT [DF_Meals_Created]  DEFAULT (sysdatetime()) FOR [Created]
GO
ALTER TABLE [dbo].[Tags] ADD  CONSTRAINT [DF_Tags_Created]  DEFAULT (sysdatetime()) FOR [Created]
GO
ALTER TABLE [dbo].[WeekPlans] ADD  CONSTRAINT [DF_WeekPlans_Created]  DEFAULT (sysdatetime()) FOR [Created]
GO
ALTER TABLE [dbo].[IngredientsInMeals]  WITH CHECK ADD  CONSTRAINT [FK_IngredientsInMeals_Ingredients] FOREIGN KEY([IngredientId])
REFERENCES [dbo].[Ingredients] ([Id])
GO
ALTER TABLE [dbo].[IngredientsInMeals] CHECK CONSTRAINT [FK_IngredientsInMeals_Ingredients]
GO
ALTER TABLE [dbo].[IngredientsInMeals]  WITH CHECK ADD  CONSTRAINT [FK_IngredientsInMeals_Meals] FOREIGN KEY([MealId])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[IngredientsInMeals] CHECK CONSTRAINT [FK_IngredientsInMeals_Meals]
GO
ALTER TABLE [dbo].[TagsOfMeals]  WITH CHECK ADD  CONSTRAINT [FK_TagsOfMeals_Meals] FOREIGN KEY([MealId])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[TagsOfMeals] CHECK CONSTRAINT [FK_TagsOfMeals_Meals]
GO
ALTER TABLE [dbo].[TagsOfMeals]  WITH CHECK ADD  CONSTRAINT [FK_TagsOfMeals_Tags] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([Id])
GO
ALTER TABLE [dbo].[TagsOfMeals] CHECK CONSTRAINT [FK_TagsOfMeals_Tags]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealFriday] FOREIGN KEY([Friday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealFriday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealMonday] FOREIGN KEY([Monday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealMonday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealSaturday] FOREIGN KEY([Saturday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealSaturday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealSunday] FOREIGN KEY([Sunday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealSunday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealThursday] FOREIGN KEY([Thursday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealThursday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealTuesday] FOREIGN KEY([Tuesday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealTuesday]
GO
ALTER TABLE [dbo].[WeekPlans]  WITH CHECK ADD  CONSTRAINT [FK_WeekPlans_MealWednesday] FOREIGN KEY([Wednesday])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[WeekPlans] CHECK CONSTRAINT [FK_WeekPlans_MealWednesday]
GO
USE [master]
GO
ALTER DATABASE [Mealplanner] SET  READ_WRITE 
GO


USE [Mealplanner]
GO

/****** Object:  Table [dbo].[BoughtIngredients]    Script Date: 11/21/2018 9:08:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BoughtIngredients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Bought] [datetime2](7) NULL,
	[WeekPlanId] [int] NOT NULL,
	[Day] [varchar](9) NULL,
	[Ingredient] [int] NULL,
 CONSTRAINT [PK_BoughtIngredients_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BoughtIngredients]  WITH CHECK ADD  CONSTRAINT [FK_BoughtIngredients_Ingredients] FOREIGN KEY([Ingredient])
REFERENCES [dbo].[Ingredients] ([Id])
GO

ALTER TABLE [dbo].[BoughtIngredients] CHECK CONSTRAINT [FK_BoughtIngredients_Ingredients]
GO

ALTER TABLE [dbo].[BoughtIngredients]  WITH CHECK ADD  CONSTRAINT [FK_BoughtIngredients_WeekPlans] FOREIGN KEY([WeekPlanId])
REFERENCES [dbo].[WeekPlans] ([Id])
GO

ALTER TABLE [dbo].[BoughtIngredients] CHECK CONSTRAINT [FK_BoughtIngredients_WeekPlans]
GO


