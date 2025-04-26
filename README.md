# StudyFlow-Arena

# OVERVIEW
StudyFlow Arena is a web application designed to help students organize their study routines and learn through interactive quizzes. It combines productivity tools with gamification and quiz battles to make learning fun, engaging, and social.<br><br>

# Development and RUN project
Run these commands to install EF Core with PostgreSQL:<br><br>

```bash

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Swashbuckle.AspNetCore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate

