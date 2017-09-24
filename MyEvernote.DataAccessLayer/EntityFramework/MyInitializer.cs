using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>


    {

        protected override void Seed(DatabaseContext context)
        {

                EvernoteUser admin = new EvernoteUser()
                {
                    Name = "Tugay",
                    Surname = "Kösekahya",
                    Email = "tugaykosekahya@gmail.com",
                    ActiveGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = true,
                    Username = "TugayKosekahya",
                    Password = "tugay5432",
                    ProfileImageFile = "user_boy.png",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now.AddMinutes(5),
                    ModifiedUsername = "TugayKosekahya"
                };

                EvernoteUser standartUser = new EvernoteUser()
                {
                    Name = "Çetin",
                    Surname = "Köse",
                    Email = "cetinkose@gmail.com",
                    ActiveGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = "CetinKose",
                    Password = "cetin5432",
                    ProfileImageFile = "user_boy.png",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now.AddMinutes(5),
                    ModifiedUsername = "TugayKosekahya"
                };

                context.EvernoteUsers.Add(admin);
                context.EvernoteUsers.Add(standartUser);

                for (int i = 0; i < 8; i++)
                {
                    EvernoteUser user = new EvernoteUser()
                    {
                        Name = FakeData.NameData.GetFirstName(),
                        Surname = FakeData.NameData.GetSurname(),
                        Email = FakeData.NetworkData.GetEmail(),
                        ProfileImageFile = "user_boy.png",
                        ActiveGuid = Guid.NewGuid(),
                        IsActive = true,
                        IsAdmin = false,
                        Username = $"user{i}",
                        Password = "123",
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = $"user{i}"
                    };

                    context.EvernoteUsers.Add(user);
                }

                context.SaveChanges();

                //Adding Fake Data
                for (int i = 0; i < 10; i++)
                {

                    Category cat = new Category()
                    {
                        Title = FakeData.PlaceData.GetStreetName(),
                        Description = FakeData.PlaceData.GetAddress(),
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        ModifiedUsername = "TugayKosekahya"
                    };

                    context.Categories.Add(cat);

                    //Adding Notes
                    for (int k = 0; k < FakeData.NumberData.GetNumber(5, 9); k++)
                    {
                        Note note = new Note()
                        {

                            Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                            Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                            Category = cat,
                            IsDraft = false,
                            LikeCount = FakeData.NumberData.GetNumber(1, 9),
                            Owner = (k % 2 == 0) ? admin : standartUser,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = (k % 2 == 0) ? admin.Username : standartUser.Username

                        };

                        cat.Notes.Add(note);

                        //Adding Fake comments
                        for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                        {
                            Comment comment = new Comment()
                            {
                                Text = FakeData.TextData.GetSentence(),
                                Owner = (j % 2 == 0) ? admin : standartUser,
                                CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                                ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                                ModifiedUsername = (j % 2 == 0) ? admin.Username : standartUser.Username

                            };

                            note.Comments.Add(comment);
                        }

                        List<EvernoteUser> userList = context.EvernoteUsers.ToList();

                        for (int m = 0; m < note.LikeCount; m++)
                        {

                            Liked liked = new Liked()
                            {
                                LikedUser = userList[m]
                            };

                            note.Likes.Add(liked);

                        }

                    }

                }

                context.SaveChanges();         

        }

    }



}
