﻿using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using UserService.AuthorizationModel;
using UserService.Db;

namespace UserService.Repo
{
    public class UserRepository(IMapper mapper, UserContext context) : IUserRepository
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserContext _context = context;

        public bool CheckUserById(Guid userId)
        {
            using (_context)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }
                return true;
            }
        }

        public Guid DeleteUser(string email)
        {
            using (_context)
            {
                var deletingUser = _context.Users.FirstOrDefault(x => x.Email == email) ?? 
                    throw new Exception($"Пользователя с почтой \"{email}\" не существует в базе");
                _context.Users.Remove(deletingUser);
                _context.SaveChanges();
                return deletingUser.Id;
            }
        }

        public Guid GetUserId(string email)
        {
            using (_context)
            {
                var user = _context.Users.First(u => u.Email == email);
                return user.Id;
            }
        }

        public IEnumerable<UserModel> GetUsers()
        {
            using (_context)
            {
                var users = _context.Users.Select(u => _mapper.Map<UserModel>(u)).ToList();
                return users;
            }
        }

        public void UserAdd(string email, string password, RoleId roleId)
        {
            using (_context)
            {
                if (roleId == RoleId.Admin)
                {
                    var c = _context.Users.Count(x => x.RoleId == RoleId.Admin);

                    if (c > 0)
                    {
                        throw new Exception("Администратор может быть только один");
                    }
                }

                var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    throw new Exception($"Пользователь с почтой \"{email}\" уже есть в базе");
                }

                var user = new User
                {
                    Email = email,
                    RoleId = roleId,
                    Salt = new byte[16]
                };
                new Random().NextBytes(user.Salt);

                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();

                user.Password = SHA512.HashData(data);

                _context.Add(user);

                _context.SaveChanges();
            }
        }

        public IdAndRoleForLogin UserCheck(string email, string password)
        {
            using (_context)
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == email) ?? throw new Exception("User not found");
                var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
                byte[] bpassword = SHA512.HashData(data);
                if (user.Password.SequenceEqual(bpassword))
                {
                    return new IdAndRoleForLogin() { Id = user.Id, RoleId = user.RoleId };
                }
                else
                {
                    throw new Exception("Wrong password");
                }
            }
        }
    }
}
