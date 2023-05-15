using Application.Models;
using Microsoft.AspNetCore.Mvc;
using NotFoundResult = Application.Models.NotFoundResult;

namespace Service.Controllers;

public abstract class BaseController : Controller
{
    protected IActionResult Result<T>(RequestResult<T> result, Func<T, object> ok)
    {
        if (result.IsValid)
            return Ok(ok(result.Result!));

        if (ReferenceEquals(result.Error, NotFoundResult.Obj))
            return base.NotFound();
        
        if (ReferenceEquals(result.Error, NotAuthorizedResult.Obj))
            return base.Unauthorized();

        return base.Problem();
    }
}