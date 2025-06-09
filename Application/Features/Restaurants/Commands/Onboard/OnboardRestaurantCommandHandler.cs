using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Restaurants.Commands.Onboard
{
    public class OnboardRestaurantCommandHandler(
        IApplicationDbContext dbContext,
        IPasswordService passwordService) : IRequestHandler<OnboardRestaurantCommand, OnboardRestaurantResult>
    {
        public async Task<OnboardRestaurantResult> Handle(OnboardRestaurantCommand command, CancellationToken cancellationToken)
        {
            // Validar que el email del administrador no exista
            var existingUser = await dbContext.Users
                .AnyAsync(u => u.Email == command.AdminEmail, cancellationToken);

            if (existingUser)
            {
                throw new Exception($"El email '{command.AdminEmail}' ya está en uso.");
            }

            // Crear la entidad Restaurante
            var newRestaurant = new Restaurant
            {
                Name = command.RestaurantName
            };

            // Hashear la contraseña
            var passwordHash = passwordService.HashPassword(command.AdminPassword);

            // Crear la entidad Usuario (Admin)
            var adminUser = new User
            {
                FullName = command.AdminFullName,
                Email = command.AdminEmail,
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                IsActive = true,
                Restaurant = newRestaurant
            };

            // Añadir las entidades al contexto
            await dbContext.Users.AddAsync(adminUser, cancellationToken);

            // Guardar los cambios
            //    Si falla, nada se guarda en la BD
            await dbContext.SaveChangesAsync(cancellationToken);

            // Devolver el resultado
            return new OnboardRestaurantResult(newRestaurant.Id, adminUser.Id);
        }
    }
}