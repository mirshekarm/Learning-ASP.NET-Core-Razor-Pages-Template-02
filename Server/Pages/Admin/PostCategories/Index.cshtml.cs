using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.PostCategories;

[Microsoft.AspNetCore.Authorization
	.Authorize(Roles = Constants.Role.Admin)]
public class IndexModel : Infrastructure.BasePageModelWithDatabaseContext
{
	#region Constractor
	public IndexModel
		(Data.DatabaseContext databaseContext,
		Microsoft.Extensions.Logging.ILogger<IndexModel> logger) :
		base(databaseContext: databaseContext)
	{
		Logger = logger;

		ViewModel =
			new System.Collections.Generic.List
			<ViewModels.Pages.Admin.PostCategories.IndexItemViewModel>();
	}
	#endregion /Constractor

	#region Public Property(ies)
	// **********
	private Microsoft.Extensions.Logging.ILogger<IndexModel> Logger { get; }
	// **********

	// **********
	public System.Collections.Generic.IList
		<ViewModels.Pages.Admin.PostCategories.IndexItemViewModel> ViewModel
	{ get; private set; }
	// **********
	#endregion /Public Property(ies)

	#region OnGetAsync
	public async System.Threading.Tasks.Task
		<Microsoft.AspNetCore.Mvc.IActionResult> OnGetAsync(System.Guid? parentId)
	{
		try
		{
			var query =
				DatabaseContext.PostCategories.AsQueryable();

			if (parentId.HasValue)
			{
				query =
					query.Where(current => current.ParentId == parentId);
			}

			ViewModel =
				await
				query
				.OrderBy(current => current.Ordering)
				.ThenBy(current => current.Title)
				.Select(current => new ViewModels.Pages.Admin.PostCategories.IndexItemViewModel
				{
					Id = current.Id,
					Title = current.Title,
					IsActive = current.IsActive,
					Ordering = current.Ordering,
					SubPostCategoriesCount = current.SubCategories.Count,
					InsertDateTime = current.InsertDateTime,
					UpdateDateTime = current.UpdateDateTime,
				})
				.ToListAsync()
				;
		}
		catch (System.Exception ex)
		{
			Logger.LogError
				(message: Constants.Logger.ErrorMessage, args: ex.Message);

			AddPageError
				(message: Resources.Messages.Errors.UnexpectedError);
		}
		finally
		{
			await DisposeDatabaseContextAsync();
		}

		return Page();
	}

	#endregion /OnGetAsync
}
