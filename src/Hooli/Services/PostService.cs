using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hooli.Models;

namespace Hooli.Services
{
    public class PostService
    {
        private Lazy<Dictionary<int, Post>> byKey;
        private Lazy<Post[]> topLevel;

        public PostService(HooliContext context)
        {
            byKey = new Lazy<Dictionary<int, Post>>(() => context.Posts.Include(u => u.User)
                .Include(g => g.Group).ToDictionary(c => c.PostId));
            topLevel = new Lazy<Post[]>(() => byKey.Value.Values.Where(c => c.ParentPostId == null).ToArray());
        }

        public Post FromKey(int postId)
        {
            return byKey.Value[postId];
        }

        public IEnumerable<Post> TopLevel()
        {
            return topLevel.Value;
        }

        public IEnumerable<Post> GetHierarchy(int postId)
        {
            var result = new List<Post>();
            var post = FromKey(postId);
            while (post != null)
            {
                result.Insert(0, post);
                post = post.ParentPost;
            }

            return result;
        }

        public IEnumerable<int> GetThisAndChildIds(int postId)
        {
            return GetAllPostIdsIncludingChildren(new Post[] { FromKey(postId) });
        }

        private static IEnumerable<int> GetAllPostIdsIncludingChildren(IEnumerable<Post> posts)
        {
            return posts
                .Select(c => c.PostId)
                .Union(posts
                    .Where(c => c.Children != null)
                    .SelectMany(c => GetAllPostIdsIncludingChildren(c.Children)));
        }
        

    }
}