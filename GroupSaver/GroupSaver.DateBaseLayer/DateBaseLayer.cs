using System;
using System.Collections.Generic;
using System.Linq;
using GroupSaver.DateBaseLayer.Model;
using GroupSaver.DateBaseLayer.Model.GroupSaver.Model;
using SQLite;
using Trace = System.Diagnostics.Trace;

namespace GroupSaver.DataBase
{
    public class DateBaseLayer
    {
        private readonly string _folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private readonly string _groupTableName = "Group.db";
        private readonly string _postTableName = "Post.db";
        private readonly string _personTableName = "Person.db";
        private readonly string _attachmentTableName = "Attachment.db";
        private readonly string _groupSubscribeTableName = "GroupSubcsribe.db";
        private readonly string _postAttachmentTableName = "PostAttachment.db";

        public bool CreateGroupTable()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(_folder, _groupTableName));
                connection.CreateTableAsync<Group>();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool CreatePersonTable()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(_folder, _personTableName));
                connection.CreateTableAsync<Person>();
                return true;
            }
            catch (Android.Database.Sqlite.SQLiteException ex)
            {
                Trace.TraceError("Datebase " + _personTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool CreateGroupSubscribeTable()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName));
                connection.CreateTableAsync<GroupSubcsribe>();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool CreatePostTable()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(_folder, _postTableName));
                connection.CreateTableAsync<Post>();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool CreateAttachmentTable()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(_folder, _attachmentTableName));
                connection.CreateTableAsync<Attachment>();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _attachmentTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool CreatePostAttachmentTable()
        {
            try
            {
                var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postAttachmentTableName));
                connection.CreateTable<PostAttachment>();
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postAttachmentTableName + " wasn't created: " + ex.Message);
                return false;
            }
        }

        public bool AddPerson(Person person)
        {
            try
            {
                if (person.FirstName != string.Empty && person.LastName != string.Empty && !HavePersonVkId(person.VkId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                    {
                        connection.Insert(person);
                        if (HavePersonVkId(person.VkId))
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": Person wasn't added. " + ex.Message);
                return false;
            }
        }

        public Person GetPersonById(int id)
        {
            try
            {
                if (HavePerson(id))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                    {
                        var persons = connection.Table<Person>().Where(i => i.Id == id);
                        if (persons.Any())
                        {
                            return new Person()
                            {
                                FirstName = persons.ElementAt(0).FirstName,
                                LastName = persons.ElementAt(0).LastName,
                                Id = id,
                                VkId = persons.ElementAt(0).VkId
                            };
                        }
                    }
                }
                throw new ArgumentException("Person with id " + id + " doesn't exist");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": Person wasn't found. " + ex.Message);
                return new Person();
            }
        }

        public bool HavePerson(int id)
        {
            try
            {
                if (id >= 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                    {
                        var users = connection.Table<Person>().Where(i => i.Id == id);
                        if (users.Any())
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException("Wrong person Id: " + id);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": Person wasn't found. " + ex.Message);
                return false;
            }
        }

        public bool HavePersonVkId(int vkId)
        {
            try
            {
                if (vkId >= 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                    {
                        if (connection.Table<Person>().Count(i => i.VkId == vkId) != 0)
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException("Wrong person VkId: " + vkId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": Person wasn't found. " + ex.Message);
                return false;
            }
        }

        public bool DeletePerson(int id)
        {
            try
            {
                if (HavePerson(id))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                    {
                        connection.Delete(GetPersonById(id));
                        if (!HavePerson(id))
                            return true;
                    }
                }
                throw new ArgumentException("Person with id " + id + "doesn't exist.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": Person wasn't deleted. " + ex.Message);
                return false;
            }
        }

        public bool DeleteAllPersons()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _personTableName)))
                {
                    var allUsers = connection.Table<Person>();
                    foreach (var user in allUsers)
                    {
                        var groups = GetAllUserGroups(user.Id);
                        foreach (var group in groups)
                        {
                            if (!DeleteSubscribe(group.Id, user.Id))
                                throw new ArgumentException("Subscribe with group id " + group.Id + " user Id " + user.Id + "wasn't deleted");
                            if (!DeleteGroup(group.Id))
                                throw new ArgumentException("Group with id " + group.Id + "wasn't deleted");
                        }

                        if (!DeletePerson(user.Id))
                            throw new ArgumentException("Person with id " + user.Id + "wasn't deleted");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _personTableName + ": All persons weren't deleted. " + ex.Message);
                return false;
            }
        }

        public bool AddGroup(Group group)
        {
            try
            {
                if (group.Name != string.Empty && group.ShortUrl != string.Empty && !HaveGroupVkId(group.VkId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        connection.Insert(group);
                        if (HaveGroupVkId(group.VkId))
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't added. " + ex.Message);
                return false;
            }
        }

        public bool HaveGroup(int id)
        {
            try
            {
                if (id > 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        var groups = connection.Table<Group>().Where(i => i.Id == id);
                        if (groups.Any())
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't found. " + ex.Message);
                return false;
            }
        }

        public bool HaveGroupVkId(int vkId)
        {
            try
            {
                if (vkId > 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        var groups = connection.Table<Group>().Where(i => i.VkId == vkId);
                        if (groups.Any())
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't found. " + ex.Message);
                return false;
            }
        }

        public Group GetGroupById(int id)
        {
            try
            {
                if (HaveGroup(id))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        var groups = connection.Table<Group>().Where(i => i.Id == id);
                        if (groups.Any())
                        {
                            return new Group()
                            {
                                Id = groups.ElementAt(0).Id,
                                Name = groups.ElementAt(0).Name,
                                ShortUrl = groups.ElementAt(0).ShortUrl,
                                VkId = groups.ElementAt(0).VkId
                            };
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't found. " + ex.Message);
                return new Group();
            }
        }

        public Group GetGroupByVkId(int vkId)
        {
            try
            {
                if (HaveGroupVkId(vkId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        var groups = connection.Table<Group>().Where(i => i.VkId == vkId);
                        if (groups.Any())
                        {
                            return new Group()
                            {
                                Id = groups.ElementAt(0).Id,
                                Name = groups.ElementAt(0).Name,
                                ShortUrl = groups.ElementAt(0).ShortUrl,
                                VkId = groups.ElementAt(0).VkId
                            };
                        }
                    }
                }
                throw new ArgumentException("Group with VkId " + vkId + " doesn't exist");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't found. " + ex.Message);
                return new Group();
            }
        }

        public bool DeleteAllGroups()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                {
                    var allGroups = connection.Table<Group>();
                    foreach (var group in allGroups)
                    {
                        var posts = GetAllGroupPosts(group.Id);
                        foreach (var post in posts)
                        {
                            if (!DeletePost(post.Id))
                                throw new ArgumentException("Post with id " + post.Id +  "wasn't deleted");
                        }

                        var subscribes = GetAllGroupSubcsribes(group.Id);
                        foreach (var subscribe in subscribes)
                        {
                            if (!DeleteSubscribe(subscribe.GroupId, subscribe.PersonId))
                                throw new ArgumentException("Subscribe with group Id " + subscribe.GroupId + " user Id" + subscribe.PersonId + "wasn't deleted");
                        }

                        if(!DeleteGroup(group.Id))
                            throw new ArgumentException("Group with Id " + group.Id + " wasn't deleted");
                    }

                    var count = connection.Table<Group>().Count();
                    if (count == 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": All groups weren't deleted. " + ex.Message);
                return false;
            }
        }

        public bool DeleteGroup(int id)
        {
            try
            {
                if (HaveGroup(id))
                {
                    using(var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        connection.Delete(GetGroupById(id));
                        if (!HaveGroup(id))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't deleted. " + ex.Message);
                return false;
            }
        }

        public bool UpdateGroup(Group groupUpdate)
        {
            try
            {
                if (groupUpdate.Name != string.Empty &&
                    groupUpdate.ShortUrl != string.Empty)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupTableName)))
                    {
                        var group = GetGroupByVkId(groupUpdate.VkId);
                        group.Name = groupUpdate.Name;
                        group.ShortUrl = groupUpdate.ShortUrl;

                        connection.Update(group);
                        return true;
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupTableName + ": Group wasn't updated. " + ex.Message);
                return false;
            }
        }

        public bool AddGroupSubscribe(int userId, int groupId)
        {
            try
            {
                if (HaveGroup(groupId) && HavePerson(userId))
                {
                    using (
                        var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName))
                    )
                    {
                        connection.Insert(new GroupSubcsribe()
                        {
                            GroupId = groupId,
                            PersonId = userId
                        });
                        return true;
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": Subscribe wasn't added. " + ex.Message);
                return false;
            }
        }

        public bool HaveSubscribe(int groupId, int userId)
        {
            try
            {
                if (HavePerson(userId) && HaveGroup(groupId))
                {
                    using (
                        var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName))
                    )
                    {
                        var res = connection.Table<GroupSubcsribe>().Where(i => i.GroupId == groupId && i.PersonId == userId);
                        if (res.Any())
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": Subscribe doesn't exist. " + ex.Message);
                return false;
            }
        }

        public GroupSubcsribe GetSubscribe(int groupId, int userId)
        {
            try
            {
                if (HaveSubscribe(groupId, userId))
                {
                    using (
                        var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName))
                    )
                    {
                        var subscribes =
                            connection.Table<GroupSubcsribe>().Where(i => i.GroupId == groupId && i.PersonId == userId);
                        if (subscribes.Any())
                        {
                            return new GroupSubcsribe()
                            {
                                Id = subscribes.ElementAt(0).Id,
                                GroupId = groupId,
                                PersonId = userId
                            };
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": Subscribe wasn't found. " + ex.Message);
                return new GroupSubcsribe();
            }
        }

        public List<GroupSubcsribe> GetAllGroupSubcsribes(int groupId)
        {
            try
            {
                if (HaveGroup(groupId))
                {
                    using (
                        var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName))
                    )
                    {
                        return connection.Table<GroupSubcsribe>().Where(i => i.GroupId == groupId).ToList();
                    }
                }
                return new List<GroupSubcsribe>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": All group subscribes weren't found" + ex.Message);
                return new List<GroupSubcsribe>();
            }
        }

        public List<Group> GetAllUserGroups(int userId)
        {
            try
            {
                if (HavePerson(userId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName)))
                    {
                        var subscribe = connection.Table<GroupSubcsribe>().Where(i => i.PersonId == userId);
                        var resGroups = new List<Group>();
                        if (subscribe.Any())
                        {
                            foreach (var group in subscribe)
                            {
                                var getGroup = GetGroupById(group.GroupId);
                                if (getGroup != new Group())
                                {
                                    resGroups.Add(getGroup);
                                }
                            }
                            return resGroups;
                        }
                    }
                }
                throw  new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": User groups wasn't found. " + ex.Message);
                return new List<Group>();
            }
        }

        public bool DeleteSubscribe(int groupId, int userId)
        {
            try
            {
                if (HaveSubscribe(groupId, userId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _groupSubscribeTableName)))
                    {
                        var subscribe = GetSubscribe(groupId, userId);
                        connection.Delete(subscribe);
                        if (!HaveSubscribe(groupId, userId))
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _groupSubscribeTableName + ": Group wasn't removed. " + ex.Message);
                return false;
            }
        }

        public bool AddPost(Post post)
        {
            try
            {
                if (HaveGroup(post.GroupId) && post.TimeDate != new DateTime() && !HavePostVkId(post.VkId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        connection.Insert(post);
                        if (HavePostVkId(post.VkId))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't added. " + ex.Message);
                return false;
            }
        }

        public bool HavePostVkId(int vkId)
        {
            try
            {
                if (vkId >= 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        if (connection.Table<Post>().Any(i => i.VkId == vkId))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't found. " + ex.Message);
                return false;
            }
        }

        public List<Post> GetAllGroupPosts(int groupId)
        {
            try
            {
                if (HaveGroup(groupId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        return connection.Table<Post>().Where(i => i.GroupId == groupId).ToList();
                    }
                }
                return new List<Post>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Group posts weren't found. " + ex.Message);
                return new List<Post>();
            }
        }

        public bool HavePostId(int id)
        {
            try
            {
                if (id >= 0)
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        if (connection.Table<Post>().Any(i => i.Id == id))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't found. " + ex.Message);
                return false;
            }
        }

        public Post GetPostById(int id)
        {
            try
            {
                if (HavePostId(id))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        var post = connection.Table<Post>().Where(i => i.Id == id);
                        if (post.Any())
                        {
                            return new Post()
                            {
                                GroupId = post.ElementAt(0).GroupId,
                                Id = id,
                                Text = post.ElementAt(0).Text,
                                TimeDate = post.ElementAt(0).TimeDate,
                                VkId = post.ElementAt(0).VkId
                            };
                        }
                    }
                }
                return new Post();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't found. " + ex.Message);
                return new Post();
            }
        }

        public Post GetPostByVkId(int vkId)
        {
            try
            {
                if (HavePostVkId(vkId))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        var post = connection.Table<Post>().Where(i => i.VkId == vkId);
                        if (post.Any())
                        {
                            return new Post()
                            {
                                GroupId = post.ElementAt(0).GroupId,
                                Id = post.ElementAt(0).Id,
                                Text = post.ElementAt(0).Text,
                                TimeDate = post.ElementAt(0).TimeDate,
                                VkId = vkId
                            };
                        }
                    }
                }
                return new Post();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't found. " + ex.Message);
                return new Post();
            }
        }

        public bool DeletePost(int id)
        {
            try
            {
                if (HavePostId(id))
                {
                    using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                    {
                        connection.Delete(GetPostById(id));
                        if (!HavePostId(id))
                        {
                            return true;
                        }
                    }
                }
                throw new ArgumentException("Post doesn't exist");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": Post wasn't deleted. " + ex.Message);
                return false;
            }
        }

        public bool DeleteAllPosts()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, _postTableName)))
                {
                    var allPosts = connection.Table<Post>();
                    foreach (var post in allPosts)
                    {
                        connection.Delete(post);
                    }

                    int res = connection.Table<Post>().Count();
                    if(res == 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Datebase " + _postTableName + ": All posts weren't deleted. " + ex.Message);
                return false;
            }
        }
    }
}