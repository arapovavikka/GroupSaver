using System;
using Android.Webkit;
using GroupSaver.DateBaseLayer.Model;
using NUnit.Framework;


namespace GroupSaver.Tests
{
    [TestFixture]
    public class DateBaseTest
    {
        private DataBase.DateBaseLayer _database;
        private Random _rnd = new Random();

        [SetUp]
        public void Setup()
        {
            _database = new DataBase.DateBaseLayer();
            _database.CreateGroupTable();
            _database.CreatePersonTable();
            _database.CreateGroupSubscribeTable();
            _database.CreatePostTable();
        }


        [TearDown]
        public void Tear() { }



        private Person GeneratePerson()
        {
            return new Person()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                VkId = _rnd.Next()
            };
        }

        private Group GenerateGroup()
        {
            return new Group()
            {
                Name = Guid.NewGuid().ToString(),
                ShortUrl = Guid.NewGuid().ToString(),
                VkId = _rnd.Next()
            };
        }

        private Post GeneratePost(int groupId)
        {
            return new Post()
            {
                GroupId = groupId,
                Text = Guid.NewGuid().ToString(),
                TimeDate = DateTime.Today,
                VkId = _rnd.Next()
            };
        }

        [Test]
        public void ShouldAddUser()
        {
            var user = GeneratePerson();

            if (_database.AddPerson(user))
            {
                if (_database.HavePersonVkId(user.VkId))
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ShouldAddGroup()
        {
            var group = GenerateGroup();

            if (_database.AddGroup(group))
            {
                if (_database.HaveGroupVkId(group.VkId))
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ShouldAddSubscribe()
        {
            var user = GeneratePerson();
            var group = GenerateGroup();
            if (_database.AddPerson(user) && _database.AddGroup(group))
            {
                _database.AddGroupSubscribe(user.Id, group.Id);
                if (_database.HaveSubscribe(group.Id, user.Id))
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ShouldAddPost()
        {
            var group = GenerateGroup();
            _database.AddGroup(group);
            group = _database.GetGroupByVkId(group.VkId);

            var post = GeneratePost(group.Id);
            _database.AddPost(post);
            if (_database.HavePostVkId(post.VkId))
                Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public void ShouldDeletePost()
        {
            var group = GenerateGroup();
            _database.AddGroup(group);
            group = _database.GetGroupByVkId(group.VkId);

            var post = GeneratePost(group.Id);
            _database.AddPost(post);
            _database.DeletePost(post.Id);

            if(_database.HavePostId(post.Id))
                Assert.Fail();
            Assert.Pass();
        }

        [Test]
        public void ShouldDeleteAllPosts()
        {
            if (_database.DeleteAllPosts())
                Assert.Pass();
            Assert.Fail();
        }

        [Test]
        public void ShouldDeleteAllGroups()
        {
            if (_database.DeleteAllGroups())
                Assert.Pass();
            Assert.Fail();
        }

    }
}