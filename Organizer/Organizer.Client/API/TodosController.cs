﻿using CefSharp.WinForms;
using Model.DataProviders;
using Organizer.Model;
using Organizer.Model.DataProviders;
using Organizer.Model.DTO;
using Organizer.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organizer.Client.API
{
    public class TodosController : AppController
    {
        private readonly GoalsProvider _goalsProvider;
        private readonly ActivitiesProvider _activitiesProvider;
        private readonly TodoItemsProvider _todoItemsProvider;
        private readonly TagsProvider _tagsProvider;

        public TodosController(ChromiumWebBrowser originalBrowser, MainWindow mainForm, DataContext dbContext) : base(originalBrowser, mainForm)
        {
            _goalsProvider = new GoalsProvider(dbContext);
            _activitiesProvider = new ActivitiesProvider(dbContext);
            _todoItemsProvider = new TodoItemsProvider(dbContext);
            _tagsProvider = new TagsProvider(dbContext);
        }

        public string GetAll(int goalId)
        {
            var data = goalId == 0 ? _todoItemsProvider.GetAll() : _todoItemsProvider.GetAll(goalId);
            return data.Select(x => new ToDoItemDto(x)).ToList().Serialize();
        }

        public string Get(int todoItemId)
        {
            var data = _todoItemsProvider.GetById(todoItemId);
            var dto = new ToDoItemDto(data);
            return dto.Serialize();
        }

        public void Delete(int todoItemId)
        {
            _todoItemsProvider.Delete(todoItemId);
            _todoItemsProvider.Save();
        }

        public void Resolve(int id, bool resolved)
        {
            _todoItemsProvider.Resolve(id, resolved);
        }

        public void Update(int id, string notes, string tags)
        {
            var tagList = new List<Tag>();
            var item = _todoItemsProvider.GetById(id);
            item.Notes = notes;

            if (string.IsNullOrEmpty(tags) || tags == " ")
            {
                item.Tags = new List<Tag>();
            }
            else
            {
                item.Tags = new List<Tag>();

                foreach (var tag in tags.Split(','))
                {
                    var dbTag = _tagsProvider.Get(tag);
                    if (dbTag != null)
                    {
                        item.Tags.Add(dbTag);
                    }
                    else
                    {
                        item.Tags.Add(new Tag
                        {
                            Name = tag,
                        });
                    }
                }
            }

            _todoItemsProvider.Update(item);
            _todoItemsProvider.Save();
        }

        public void Add(string description, DateTime deadline, int activityId, int duration, bool resolved = false)
        {
            _todoItemsProvider.Insert(new TodoItem
            {
                ActivityId = activityId,
                Deadline = deadline,
                AddedOn = DateTime.Now,
                Description = description,
                Duration = duration,
                Resolved = resolved,
            });

            var activity = _activitiesProvider.GetById(activityId);
            if (activity != null && activity.StartDate == null)
            {
                var date = activity.TodoItems.Min(t => t.AddedOn);
                activity.StartDate = date;
                var goal = _goalsProvider.GetById(activity.GoalId);
                if (goal != null && goal.Start == null)
                {
                    goal.Start = date;
                }
            }

            _todoItemsProvider.Save();
        }

        public void UpdateDescription(int id, string value)
        {
            var todo = _todoItemsProvider.GetById(id);
            if (!todo.Description.Equals(value))
            {
                todo.Description = value;
                _todoItemsProvider.Save();
            }
        }

        public void UpdateDuration(int id, int value)
        {
            var todo = _todoItemsProvider.GetById(id);
            if (todo.Duration != value)
            {
                todo.Duration = value;
                _todoItemsProvider.Save();
            }
        }

        public string GetTags()
        {
            var data = _tagsProvider.GetAll().Select(tag => new TagDto(tag));
            return data.Serialize();
        }

        public string GetTagNames()
        {
            var data = _tagsProvider.GetAll().Select(x => x.Name).ToList();
            return data.Serialize();
        }
    }
}
