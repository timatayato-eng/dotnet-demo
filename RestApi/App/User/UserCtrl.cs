using Microsoft.AspNetCore.Mvc;
using RestApi.App.User.Req;
using RestApi.Models;

namespace RestApi.App.User;

[ApiController]
[Tags("User")]
[Route("user")]
public class UserCtrl(UserService service) : ControllerBase
{
  private readonly UserService _service = service;

  [HttpGet("/users")]
  public async Task<ActionResult<List<UserModel>>> FindAll()
  {
    return await _service.FindAll();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<UserModel>> FindById(int id)
  {
    var user = await _service.FindById(id);
    if (user is null) return NotFound();
    return user;
  }

  [HttpPost]
  public async Task<IActionResult> Save([FromBody] UserSaveReq form)
  {
    try
    {
      var user = await _service.Save(form);
      return Created($"/user/{user.Id}", user);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, [FromBody] UserSaveReq form)
  {
    var user = await _service.Update(id, form);
    if (user is null) return NotFound();
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Remove(int id)
  {
    var removed = await _service.Remove(id);
    if (!removed) return NotFound();
    return NoContent();
  }
}
