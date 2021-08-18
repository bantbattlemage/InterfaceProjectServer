using GameComms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GameServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LogInController : Controller
	{
		public static Dictionary<string, GameUser> ActiveUsers = new Dictionary<string, GameUser>();

		[HttpGet("{id}")]
		public JsonResult Get(int id)
		{
			GetUserResponse response = new GetUserResponse();

			string sql = $"SELECT * FROM Users.Accounts WHERE id={id}";
			JsonResult result = SqlConnector.Query(sql);
			GameUser gameUser = JsonConvert.DeserializeObject<GameUser>((string)result.Value);

			response.Message = "success";
			response.Success = true;
			response.User = gameUser;

			return new JsonResult(response);
		}

		[HttpPost]
		public JsonResult Post([FromBody] LogInRequest request)
		{
			LogInResponse response = new LogInResponse();

			if(request == null || request.Username == "" || request.Password == "")
			{
				response.Success = false;
				response.Message = "Null request";

				return new JsonResult(response);
			}

			string user = SqlConnector.SqlString(request.Username);
			string pass = request.Password;
			string email = SqlConnector.SqlString(request.Email);
			bool usernameExists = false;
			bool emailExists = false;

			string sql = $"SELECT username FROM Users.Accounts WHERE username={user}";
			JsonResult query = SqlConnector.Query(sql);
			if(query != null)
			{
				usernameExists = (string)query.Value != "";
			}

			sql = $"SELECT email FROM Users.Accounts WHERE email={email}";
			query = SqlConnector.Query(sql);
			if (query != null)
			{
				emailExists = (string)query.Value != "";
			}

			if (!request.NewRegistration && !usernameExists)
			{
				response.Success = false;
				response.Message = "Username does not exist";
			}
			else if(request.NewRegistration && usernameExists)
			{
				response.Success = false;
				response.Message = "Username already exits";
			}
			else if(request.NewRegistration && emailExists)
			{
				response.Success = false;
				response.Message = "Email already registered";
			}
			else if (request.NewRegistration)
			{
				if(request.Email == "")
                {
					response.Success = false;
					response.Message = "Invalid email";
					return Json(response);
				}

				pass = PasswordHasher.HashPassword(pass);
				pass = SqlConnector.SqlString(pass);

				sql = $"INSERT INTO Users.Accounts VALUES ({user}, {pass}, {email}, SYSDATETIME())";
				
				JsonResult result = SqlConnector.Query(sql);

				if(result != null)
				{
					response.Success = false;
					response.Message = "New account successfully registered";
				}
				else
				{
					response.Success = false;
					response.Message = "Account creation failed";
				}
			}
			else
			{
				if(!usernameExists)
				{
					response.Success = false;
					response.Message = "Username does not exist";
				}
				else
				{
					sql = $"SELECT * FROM Users.Accounts WHERE username={user}";
					query = SqlConnector.Query(sql);
					GameUser gameUser = JsonConvert.DeserializeObject<GameUser>((string)query.Value);
					string pw = gameUser.Password;

					if (PasswordHasher.VerifyHashedPassword(pw, pass))
					{
						response.Success = true;
						response.Message = "Welcome back";
						//ActiveUsers.Add(pw, gameUser);
						Console.WriteLine($"{gameUser.Username} has logged in.");
					}
					else
					{
						response.Success = false;
						response.Message = "Incorrect password";
					}
				}
			}

			return new JsonResult(response);
		}

		[HttpPut("{id}")]
		public JsonResult Put(int id, [FromBody] string value)
		{
			return Json(false);
		}

		[HttpDelete("{id}")]
		public JsonResult Delete(int id)
		{
			return Json(false);
		}
	}
}