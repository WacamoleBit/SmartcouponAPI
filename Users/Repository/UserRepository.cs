﻿using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartcouponAPI.Context.Identity.UserIdentity;
using SmartcouponAPI.Users.Model;
using SmartcouponAPI.Users.Model.Requests;
using SmartcouponAPI.Users.Model.Responses;
using System.Text;

namespace SmartcouponAPI.Users.Repository
{
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Valida y registra un nuevo User
        /// </summary>
        /// <param name="request">Datos para registro de un usuario.</param>
        /// <param name="_userManager">Instancia de UserManager. Para guardar el usuario</param>
        /// <param name="_context">Instancia de contexto. Para guardar los datos del usuario</param>
        /// <returns>UserRegisterResponse, si se completó exitosamente el registro, UserName es diferente de null</returns>
        public async Task<UserRegisterResponse> Register(UserRegisterRequest request, UserManager<User> _userManager, UserIdentityDbContext _context)
        {
            StringBuilder message = new StringBuilder();

            UserRegisterResponse response = new UserRegisterResponse()
            {
                Message = String.Empty,
                UserName = null
            };

            using (var transaction = await _context.Database.BeginTransactionAsync()) // Uso de trasactions
            {
                try
                {
                    User newUser = new User()
                    {
                        UserName = request.UserName,
                        PasswordHash = request.Password,
                        Email = request.Email
                    };

                    IdentityResult? userCreationResult = await _userManager.CreateAsync(newUser, newUser.PasswordHash);

                    if (!userCreationResult.Succeeded)
                    {
                        message.AppendLine("Error al crear usuario:");

                        foreach (var error in userCreationResult.Errors)
                        {
                            message.AppendLine(error.Description);
                        }

                        response.Message = message.ToString();
                        await transaction.RollbackAsync();
                        return response;
                    }

                    UserData newUserData = new UserData()
                    {
                        UserName = newUser.UserName,
                        Name = request.Name,
                        FatherLastName = request.FatherLastName,
                        MotherLastName = request.MotherLastName,
                        CURP = request.CURP
                    };

                    await _context.UserData.AddAsync(newUserData);
                    var affectedRows = _context.SaveChanges();

                    if (affectedRows < 1)
                    {
                        message.AppendLine("Error al conectar con la base de datos. Inténtelo más tarde.");

                        response.Message = message.ToString();
                        await transaction.RollbackAsync();
                        return response;
                    }

                    await transaction.CommitAsync();

                    message.AppendLine("Usuario creado con éxito");

                    response.Message = message.ToString();
                    response.UserName = newUser.UserName;

                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.Message = "Ocurrió un error al procesar la solicitud. Por favor, inténtelo más tarde.";
                    return response;
                }
            }
        }
    }


}