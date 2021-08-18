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

			string user = $"'{request.Username}'";
			string userTypedPassword = request.Password;
			string email = $"'{request.Email}'";
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
				GetUserResponse r;
				GameUser newUser = RegisterNewAccount(request, out r);
				if(newUser != null)
				{
					string sessionKey = CreateNewLogInSession(newUser);
					response.Success = true;
					response.Message = r.Message;
					response.AccessKey = sessionKey;
					return Json(response);
				}
				else
				{
					response.Success = r.Success;
					response.Message = r.Message;
					return Json(response);
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
					string hashedPassword = gameUser.Password;

					if (PasswordHasher.VerifyHashedPassword(hashedPassword, userTypedPassword))
					{
						string accessKey = CreateNewLogInSession(gameUser);

						if(accessKey != null)
						{
							response.AccessKey = accessKey;
							response.Success = true;
							response.Message = "Welcome back";
							Console.WriteLine($"{gameUser.Username} has logged in.");
						}
						else
						{
							response.Success = false;
							response.Message = "Session creation failed";
						}
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

		private static string CreateNewLogInSession(GameUser user)
		{
			string accesskey = PasswordHasher.HashPassword(user.Password);
			string accessToken = PasswordHasher.HashPassword(accesskey);
			//bool match = PasswordHasher.VerifyHashedPassword(accessToken, accesskey);
			//Console.WriteLine(accesskey);

			string sql = $"DELETE FROM Users.LogInSessions WHERE userId={user.Id} INSERT INTO Users.LogInSessions VALUES({user.Id}, '{user.Username}', '{accessToken}', SYSDATETIME()) SELECT * FROM Users.LogInSessions WHERE id=SCOPE_IDENTITY()";
			JsonResult query = SqlConnector.Query(sql);
			LogInSession newSession = JsonConvert.DeserializeObject<LogInSession>((string)query.Value);

			if(newSession != null)
			{
				return accesskey;
			}
			else
			{
				return null;
			}
		}

		private GameUser RegisterNewAccount(LogInRequest request, out GetUserResponse response)
		{
			response = new GetUserResponse();

			if (request.Email == "")
			{
				response.Success = false;
				response.Message = "Invalid email";
			}

			string hashedPassword = PasswordHasher.HashPassword(request.Password);
			string sql = $"INSERT INTO Users.Accounts VALUES ('{request.Username}', '{hashedPassword}', '{request.Email}', SYSDATETIME()) SELECT * FROM Users.Accounts WHERE id=SCOPE_IDENTITY()";
			JsonResult result = SqlConnector.Query(sql);
			GameUser newUser = JsonConvert.DeserializeObject<GameUser>((string)result.Value);

			if (result != null)
			{
				response.Success = false;
				response.Message = "New account successfully registered";
			}
			else
			{
				response.Success = false;
				response.Message = "Account creation failed";
			}

			return newUser;
		}
	}
}