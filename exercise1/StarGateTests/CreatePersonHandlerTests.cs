using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;

namespace StarGateTests
{
  public class CreatePersonHandlerTests
  {


    [Fact]
    public async Task Handle_CreatesNewPerson_AndReturnsId()
    {
      var ctx = TestDbFactory.CreateSqliteInMemoryContext();

      var handler = new CreatePersonHandler(ctx);

      var uniqueName = "Person_" + Guid.NewGuid().ToString();

      var req = new CreatePerson { Name = uniqueName };

      var result = await handler.Handle(req, CancellationToken.None);

      Assert.NotNull(result);
      Assert.True(result.Id > 0);

      var created = ctx.People.FirstOrDefault(p => p.Id == result.Id);
      Assert.NotNull(created);
      Assert.Equal(uniqueName, created.Name);
    }

    [Fact]
    public async Task Preprocessor_Throws_WhenPersonAlreadyExists()
    {
      var ctx = TestDbFactory.CreateSqliteInMemoryContext();

      var pre = new CreatePersonPreProcessor(ctx);

      // Seed includes "John Doe"
      var req = new CreatePerson { Name = "John Doe" };

      await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
    }
  }
}
