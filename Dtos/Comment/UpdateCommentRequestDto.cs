using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Comment
{
    public class UpdateCommentRequestDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be min 5 characters")]
        [MaxLength(200, ErrorMessage = "Title must be max 200  characters")]

        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Title must be min 5 characters")]
        [MaxLength(200, ErrorMessage = "Title must be max 200  characters")]

        public string Content { get; set; } = string.Empty;

    }
}
