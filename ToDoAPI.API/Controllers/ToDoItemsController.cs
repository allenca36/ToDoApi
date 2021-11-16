using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.DATA.EF;
using ToDoAPI.API.Models;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoItemsController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //READ Functionality
        //api/ToDoItems
        public IHttpActionResult GetToDoItems()
        {
            List<TodoItemViewModel> toDoItems = db.TodoItems.Include("Category").Select(t => new TodoItemViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId == null ? 0 : t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList<TodoItemViewModel>();

            if (toDoItems.Count == 0)
            {
                return NotFound();
            }
            return Ok(toDoItems);
        }

        public IHttpActionResult GetToDoItem(int id)
        {
            TodoItemViewModel toDoItem = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t => new TodoItemViewModel()
            {
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }

            }).FirstOrDefault();

            if (toDoItem == null)
            {
                return NotFound();
            }
            return Ok(toDoItem);
        }

        //api/ToDoItems (HttpPost)
        public IHttpActionResult PostToDoItem(TodoItemViewModel toDoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem newItem = new TodoItem()
            {
                TodoId = toDoItem.TodoId,
                Action = toDoItem.Action,
                Done = toDoItem.Done,
                CategoryId = toDoItem.CategoryId
            };

            db.TodoItems.Add(newItem);
            db.SaveChanges();
            return Ok(newItem);
        }//end PostItem

        //api/TodoItems (HttpPut)
        public IHttpActionResult PutItem(TodoItemViewModel item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem existingItem = db.TodoItems.Where(t => t.TodoId == item.TodoId).FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.TodoId = item.TodoId;
                existingItem.Action = item.Action;
                existingItem.Done = item.Done;
                existingItem.CategoryId = item.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end PutResource

        //api/TodoItems (HttpDelete)
        public IHttpActionResult DeleteItem(int id)
        {
            TodoItem item = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();
            if (item != null)
            {
                db.TodoItems.Remove(item);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteResource

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }//end clas
}//end namespace
