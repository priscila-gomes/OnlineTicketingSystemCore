SET IDENTITY_INSERT [dbo].[Tickets] ON
INSERT INTO [dbo].[Tickets] ([TicketId], [ProjectRefId], [DeptRefId], [EmpRefId], [Description], [SubmitDate], [Status]) VALUES (1, 4, 110, 18, N'Description du problème3', N'2020-12-18 19:29:07', N'C')
INSERT INTO [dbo].[Tickets] ([TicketId], [ProjectRefId], [DeptRefId], [EmpRefId], [Description], [SubmitDate], [Status]) VALUES (2, 1, 107, 15, N'Problem Description', N'2020-12-18 19:59:09', N'O')
INSERT INTO [dbo].[Tickets] ([TicketId], [ProjectRefId], [DeptRefId], [EmpRefId], [Description], [SubmitDate], [Status]) VALUES (3, 3, 111, 4, N'Problem Description3', N'2019-12-18 19:59:39', N'O')
INSERT INTO [dbo].[Tickets] ([TicketId], [ProjectRefId], [DeptRefId], [EmpRefId], [Description], [SubmitDate], [Status]) VALUES (5, 1, 110, 37, N'Problem Description', N'2020-12-18 20:00:21', N'O')
INSERT INTO [dbo].[Tickets] ([TicketId], [ProjectRefId], [DeptRefId], [EmpRefId], [Description], [SubmitDate], [Status]) VALUES (6, 3, 109, 1, N'Problem Description222', N'2019-12-18 20:00:41', N'C')
SET IDENTITY_INSERT [dbo].[Tickets] OFF
