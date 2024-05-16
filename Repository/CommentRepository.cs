using api.Data;
using api.Dtos.Stock;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _contex;
        public CommentRepository(ApplicationDBContext contex )
        {
            _contex = contex;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _contex.Comments.AddAsync( commentModel );
            await _contex.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel= await _contex.Comments.FirstOrDefaultAsync( c => c.Id == id );
            if ( commentModel == null ) { return null; }
            _contex.Comments.Remove( commentModel );
            await _contex.SaveChangesAsync();
            return commentModel;

        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _contex.Comments.Include(c=>c.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _contex.Comments.Include(c => c.AppUser).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment= await _contex.Comments.FindAsync( id);
            if (existingComment == null) { return null; }
            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;
           
            await _contex.SaveChangesAsync();
            return existingComment;
        }
    }
}
