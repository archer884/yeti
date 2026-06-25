using Microsoft.AspNetCore.Mvc;

using Yeti.Core;

namespace Yeti.Api.Controller;

public class YetiController : ControllerBase
{
    protected long UserId => User.GetUserId() ?? throw new UnauthorizedAccessException();
}
