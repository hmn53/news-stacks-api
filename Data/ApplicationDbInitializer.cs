using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NewsStacksAPI.Models;
using System;
using System.Linq;

namespace NewsStacksAPI.Data
{
    public class ApplicationDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider
                    .GetService<ApplicationDbContext>();

                if (context.Accounts.Any() && context.Articles.Any())
                {
                    return;
                }
                else
                {
                    Account accountWriter = new Account
                    {
                        Username = "john",
                        Password = "john123",
                        Role = "Writer"
                    };
                    Account accountPublisher = new Account
                    {
                        Username = "sam",
                        Password = "sam123",
                        Role = "Publisher"
                    };
                    Account accountReader = new Account
                    {
                        Username = "raj",
                        Password = "raj123",
                        Role = "Reader"
                    };

                    context.Accounts.AddRange(accountPublisher, accountReader, accountWriter);
                    context.SaveChanges();


                    Writer writer = new Writer
                    {
                        Name = "John Smith",
                        Email = "john@gmail.com",
                        Account = accountWriter
                    };

                    context.Writers.Add(writer);
                    context.SaveChanges();


                    Publisher publisher = new Publisher
                    {
                        Name = "Sam Carlos",
                        Email = "sam@gmail.com",
                        Account = accountPublisher
                    };

                    context.Publishers.Add(publisher);
                    context.SaveChanges();


                    Tag tagone = new Tag
                    {
                        Title = "World"
                    };

                    Tag tagtwo = new Tag
                    {
                        Title = "Covid19"
                    };

                    context.Tags.AddRange(tagone, tagtwo);
                    context.SaveChanges();


                    Article articleNotPublished = new Article()
                    {
                        Headline = "A popular vacation destination in India faces a devastating surge of infection.",
                        Description = "Just a few months ago, the southwestern state of Goa was welcoming tourists from across the rest of the country. Now it is in the grip of a major outbreak.",
                        Body = "Just a few months ago, the southwestern state of Goa was welcoming tourists from across the rest of India who were drawn to its picture-perfect beaches, an ideal source of relief from coronavirus rules in other regions.Group celebrations, many without masks, were common. Life appeared to have gone back to normal.But it did not last.With India in the grip of a devastating coronavirus outbreak, 26 people died at the state-run Goa Medical College and Hospital on Tuesday morning, possibly because of an oxygen shortage, one official said.",
                        CreatedAt = new DateTime(2021, 05, 14),
                        CreatedBy = writer.Id,
                        LastModified = new DateTime(2021, 05, 15),
                        LastModifiedBy = writer.Id,
                        IsSubmitted = false,
                        Tags = new()
                        {
                            tagone,
                            tagtwo
                        },
                        Writers = new()
                        {
                            writer
                        },
                    };

                    Article articlePublished = new Article()
                    {
                        Headline = "Israeli-Palestinian Strife Widens as Frantic Calls for Calm Go Unheeded",
                        Description = "While Israel and Hamas signaled willingness to consider a cease-fire, worries grew about further fracturing in one of the Middle East’s most intractable struggles.",
                        Body = "CAIRO — Violence between Israelis and Palestinians expanded in new directions on Friday, with deadly clashes convulsing the occupied West Bank and anti-Israeli protests erupting along Israel’s borders with two Arab neighbors.The widening sense of mayhem in Israel and the Palestinian territories came as  Israeli airstrikes brought mass evacuations and funerals to Gaza, and as Hamas rockets singed Israeli towns for a fifth consecutive day.",
                        CreatedAt = new DateTime(2021, 05, 14),
                        CreatedBy = writer.Id,
                        LastModified = new DateTime(2021, 05, 15),
                        LastModifiedBy = writer.Id,
                        IsSubmitted = true,
                        SubmittedAt = new DateTime(2021, 05, 15),
                        SubmittedBy = writer.Id,
                        MetaData = "meta data-rh='true' property='article:published_time' content='2021 - 05 - 14T22: 42:22.000Z'/><meta data-rh='true' property='article: modified_time' content='2021 - 05 - 15T01: 00:40.255Z'/><meta data-rh='true' http-equiv='Content - Language' content='en'/><meta data-rh='true' name='robots' content='noarchive'/>",
                        IsPublished = true,
                        PublishedAt = new DateTime(2021, 05, 16),
                        PublishedBy = publisher.Id,
                        Tags = new()
                        {
                            tagone
                        },
                        Writers = new()
                        {
                            writer
                        },
                        Publishers = new()
                        {
                            publisher
                        }
                    };

                    context.Articles.AddRange(articleNotPublished, articlePublished);

                    context.SaveChanges();

                }
            }
        }
    }
}
