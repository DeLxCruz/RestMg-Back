using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender mediator) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var command = new LoginCommand(request.Email, request.Password);
                var authResult = await mediator.Send(command);
                return Ok(authResult);
            }
            catch (Exception ex)
            {
                // En un caso real, podrías tener excepciones personalizadas
                // para devolver 401 Unauthorized o 400 Bad Request.
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}