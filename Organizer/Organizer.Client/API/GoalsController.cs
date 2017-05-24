﻿using CefSharp.WinForms;
using Model.DataProviders;
using Organizer.Model;
using Organizer.Model.DTO;
using Organizer.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organizer.Client.API
{
    public class GoalsController : AppController
    {
        private GoalsProvider _goalsProvider;
        private ActivitiesProvider _acivitiesProvider;
        private TodoItemsProvider _todoItemsProvider;
        private DataContext _dbContext;

        public GoalsController(ChromiumWebBrowser originalBrowser, MainWindow mainForm) : base(originalBrowser, mainForm)
        {
            _dbContext = new DataContext();
            Setup(_dbContext);
        }

        public GoalsController() { }

        public void Setup(DataContext dbContext) {
            _goalsProvider = new GoalsProvider(dbContext);
            _acivitiesProvider = new ActivitiesProvider(dbContext);
            _todoItemsProvider = new TodoItemsProvider(dbContext);
        }

        #region Goals

        public string GetAll()
        {
            var data = _goalsProvider.GetAll();
            return data.Serialize();
        }

        public Goal Get(int id)
        {
            var goal = _goalsProvider.GetById(id);
            _dbContext.Entry<Goal>(goal).Reload();

            return goal;
        }

        public void Add(string name, int priority)
        {
            var id = new Random().Next(100000);
            _goalsProvider.Insert(new Goal
            {
                Name = name,
                Priority = priority,
                MinHoursPerWeek = 2,
                MaxHoursPerWeek = 8
            });
            _goalsProvider.Save();
        }

        public void Delete(int id)
        {
            var goal = Get(id);
            goal.Activities.ToList().ForEach(a => {
                a.TodoItems.ToList().ForEach(t => _todoItemsProvider.Delete(t.Id));
                _acivitiesProvider.Delete(a.Id);
            });

            _goalsProvider.Delete(goal);
            _goalsProvider.Save();
        }

        public void UpdatePriority(int id, int newPriority)
        {
            _goalsProvider.UpdatePriority(id, newPriority);
        }

        public void UpdateSetting(int id, int minHoursPerWeek, int maxHoursPerWeek)
        {
            _goalsProvider.UpdateHours(id, (short)minHoursPerWeek, (short)maxHoursPerWeek);
        }

        #endregion

        #region Activities

        public void SaveActivity(int goalId, string name, int activityId)
        {
            if (activityId == 0)
            {
                _acivitiesProvider.Insert(new Activity
                {
                    Name = name,
                    Description = null,
                    GoalId = goalId,
                    Priority = 2
                });
            }
            else
            {
                var activity = _acivitiesProvider.GetById(activityId);
                activity.Name = name;
                _acivitiesProvider.Update(activity);
            }
            _acivitiesProvider.Save();
        }

        public void DeleteActivity(int id)
        {
            var activity = _acivitiesProvider.GetById(id);
            activity.TodoItems.ToList().ForEach(t => _todoItemsProvider.Delete(t.Id));

            _acivitiesProvider.Delete(activity.Id);
            _acivitiesProvider.Save();
        }

        public string GetActivityItems(int goalId)
        {
            var dictionary = new Dictionary<int, string>();
            return _acivitiesProvider.GetAll(goalId).Select(x => new ActivityDto()
            {
                Name = x.Name,
                Id = x.Id
            }).ToList().Serialize();
        }

        #endregion
    }
}