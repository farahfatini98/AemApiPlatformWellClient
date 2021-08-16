using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Cache;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.Json;


namespace PlatformWellApiConsumption
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            try
            {
                Console.Write("-----AEMEnersol API Login----");
                IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().AddCommandLine(args).Build();
                Console.Write("Username = ");
                String Username = Console.ReadLine();
                Console.Write("Password = ");
                String Password = Console.ReadLine();
                var token = await Login(Username, Password);
                Console.WriteLine("Token = {0}",token);
                //string AuthToken = token.Trim('\"').ToString();
                var PlatformWell= await GetPlatformWellActual(token);
                var context = new AemModels.AEMENERSOLContext();
                foreach(var i in PlatformWell)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Platform and Well Data =");
                    Console.WriteLine("");
                    Console.WriteLine("Platform Id : "+i.id);
                    Console.WriteLine("Platform Name : "+i.uniqueName);
                    Console.WriteLine("Platform Latitude : " + i.latitude);
                    Console.WriteLine("Platform Longitude : " + i.longitude);
                    Console.WriteLine("Platform Created At : " + i.createdAt);
                    Console.WriteLine("Platform Updated At : " + i.updatedAt);


                    var ToInsertPlatform = new AemModels.Platform
                    {
                        Id = i.id,
                        UniqueName = i.uniqueName,
                        Latitude = i.latitude,
                        Longitude = i.longitude,
                        CreatedAt = DateHandler(i.createdAt),
                        UpdatedAt = DateHandler(i.updatedAt),



                     };


                    var CheckDataExists = context.Platforms.Where(x => x.Id == i.id).FirstOrDefault();
                    if (CheckDataExists == null)
                    {
                        await context.Platforms.AddAsync(ToInsertPlatform);
                        await context.SaveChangesAsync();
                        if (i.well != null)
                        {
                            

                            foreach (var well in i.well)
                            {
                                
                                var ToInsertWell = new AemModels.Well
                                {
                                    Id = well.id,
                                    PlatformId = i.id,
                                    UniqueName = well.uniqueName,
                                    Latitude = well.latitude,
                                    Longitude = well.longitude,
                                    CreatedAt = DateHandler(well.createdAt),
                                    UpdatedAt = DateHandler(well.updatedAt)
                                };

                                await context.Wells.AddAsync(ToInsertWell);
                                await context.SaveChangesAsync();

                            }
                        }
                    }
                    else 
                    {

                        CheckDataExists.Id = i.id;
                        CheckDataExists.UniqueName = i.uniqueName;
                        CheckDataExists.Latitude = i.latitude;
                        CheckDataExists.Longitude = i.longitude;
                        CheckDataExists.CreatedAt = DateHandler(i.createdAt);
                        CheckDataExists.UpdatedAt = DateHandler(i.updatedAt);
                        await context.SaveChangesAsync();
                        if (i.well != null)
                        {
                            foreach (var well in i.well)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Well Id : " + well.id);
                                Console.WriteLine("Platform Id : "+i.id);
                                Console.WriteLine("Well Name : " + well.uniqueName);
                                Console.WriteLine("Well Latitude : " + well.latitude);
                                Console.WriteLine("Well Longitude : " + well.longitude);
                                Console.WriteLine("Well Created At : " + well.createdAt);
                                Console.WriteLine("Well Updated At : " + well.updatedAt);

                                var CheckWellExist = context.Wells.Where(x => x.Id == well.id).FirstOrDefault();
                                if (CheckWellExist != null) 
                                {
                                    CheckWellExist.Id = well.id;
                                    CheckWellExist.PlatformId = i.id;
                                    CheckWellExist.UniqueName = well.uniqueName;
                                    CheckWellExist.Latitude = well.latitude;
                                    CheckWellExist.Longitude = well.longitude;
                                    CheckWellExist.CreatedAt = DateHandler(well.createdAt);
                                    CheckWellExist.UpdatedAt = DateHandler(well.updatedAt);
                                    await context.SaveChangesAsync();
                                } else {
                                    var ToInsertWell = new AemModels.Well
                                    {
                                        Id = well.id,
                                        PlatformId = i.id,
                                        UniqueName = well.uniqueName,
                                        Latitude = well.latitude,
                                        Longitude = well.longitude,
                                        CreatedAt = DateHandler(well.createdAt),
                                        UpdatedAt = DateHandler(well.updatedAt)
                                    };

                                    await context.Wells.AddAsync(ToInsertWell);
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                    }

                   
                    


                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        public static DateTime DateHandler(DateTime Date) 
        {
            DateTime CreatedAtout;
;
            if (DateTime.TryParse(Date.ToString(), out CreatedAtout))
            {

                if (Date < SqlDateTime.MinValue.Value || Date > SqlDateTime.MaxValue.Value)
                { CreatedAtout = DateTime.UtcNow; }
                else { return CreatedAtout = Date; }

            }
            else { return CreatedAtout = DateTime.UtcNow; }
            return new DateTime(2021, 10,10);
        }

  

        public static async Task<string> Login(string userName, string password) //userName -->camelCase UserName-->PascalCase
        {
            try {
                var LoginCred = new UserLogin
                {
                    username = userName,
                    password = password
                };
                HttpClient client = new HttpClient();
                var DeserializedLoginCred = JsonSerializer.Serialize(LoginCred);
                client.BaseAddress = new Uri("http://test-demo.aemenersol.com/");
                var response = await client.PostAsync("api/Account/Login", new StringContent(DeserializedLoginCred.ToString(), System.Text.Encoding.UTF8, "application/json"));
                string responseString = await response.Content.ReadAsStringAsync();
                return "bearer " + responseString.Trim('"').ToString();

            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
        public static async Task<List<PlatformWellApiConsumption.Platform>> GetPlatformWellActual(string AuthToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", AuthToken);
            client.BaseAddress = new Uri("http://test-demo.aemenersol.com/");
            var StreamResponse = client.GetStreamAsync("/api/PlatformWell/GetPlatformWellDummy");
            var repositories = await JsonSerializer.DeserializeAsync<List<PlatformWellApiConsumption.Platform>>(await StreamResponse);
            return repositories;
        }

    } 
}

