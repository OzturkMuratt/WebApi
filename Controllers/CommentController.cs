using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mapper;
using api.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace api.Controllers
{
    [Route("api/Comment")]
    [ApiController]
    public class CommentController:ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(
            ICommentRepository commentRepo,
            IStockRepository stockRepository,
            UserManager<AppUser> userManager
            )
        {
            _commentRepo = commentRepo;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) {return BadRequest(ModelState);}
            var comments= await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(x => x.ToCommentDto());
            return Ok(commentDto);
         }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByID([FromRoute] int id)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment==null) { return NotFound();}
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId,CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            if (!await _stockRepository.StockExist(stockId)) { return BadRequest("Stock does not exist"); }

            var username= User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = user.Id;

            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetByID), new { id=commentModel.Id},commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto) 
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if (comment==null)
            {
              return  NotFound("Comment Not Found");

            }
            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null) { return NotFound("Comment Does Not EXIST"); }
            return Ok(commentModel);

        }
    }

}
