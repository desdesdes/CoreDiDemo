using NUnit.Framework;
using Afas.Bvr.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Afas.Bvr.Core.Repository;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace Afas.Bvr.Crm.Tests;

[TestFixture()]
public class WebPhonenumberCheckerTests
{
  [Test()]
  public void CheckPhoneNumber_WhenServiceReturnTrue_ReturnsTrue()
  {
    var _mockMessageHandler = A.Fake<HttpMessageHandler>();

    A.CallTo(_mockMessageHandler)
    .WithReturnType<Task<HttpResponseMessage>>()
    .Returns(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent("true")
    });

    var checker = new WebPhonenumberChecker(new HttpClient(_mockMessageHandler));

    Assert.That(() => checker.CheckPhoneNumber("(06) 11"), Is.True);
  }

  [Test()]
  public void CheckPhoneNumber_WhenServiceReturnFalse_ReturnsFalse()
  {
    var _mockMessageHandler = A.Fake<HttpMessageHandler>();

    A.CallTo(_mockMessageHandler)
    .WithReturnType<Task<HttpResponseMessage>>()
    .Returns(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent("false")
    });

    var checker = new WebPhonenumberChecker(new HttpClient(_mockMessageHandler));

    Assert.That(() => checker.CheckPhoneNumber("(06) 11"), Is.False);
  }
}
