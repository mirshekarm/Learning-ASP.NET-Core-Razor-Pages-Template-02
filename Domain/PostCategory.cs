﻿namespace Domain
{
	public class PostCategory :
		SeedWork.Entity,
		SeedWork.IEntityHasIsActive,
		SeedWork.IEntityHasUpdateDateTime
	{
		#region Constant(s)
		public const byte DescriptionMaxLength = 100;
		#endregion /Constant(s)

		#region Constructor
		public PostCategory(string title) : base()
		{
			Title = title;

			UpdateDateTime = InsertDateTime;

			SubCategories =
				new System.Collections.Generic.List<PostCategory>();
		}
		#endregion /Constructor(s)

		#region Property(ies)
		// **********
		// **********
		// **********
		/// <summary>
		/// والد
		/// </summary>
		[System.ComponentModel.DataAnnotations.Display
			(Name = nameof(Resources.DataDictionary.Parent),
			ResourceType = typeof(Resources.DataDictionary))]
		public virtual PostCategory? Parent { get; set; }
		// **********

		// **********
		[System.ComponentModel.DataAnnotations.Display
			(Name = nameof(Resources.DataDictionary.Parent),
			ResourceType = typeof(Resources.DataDictionary))]
		public System.Guid? ParentId { get; set; }
		// **********
		// **********
		// **********

		// **********
		/// <summary>
		/// عنوان
		/// </summary>
		[System.ComponentModel.DataAnnotations.Display
			(ResourceType = typeof(Resources.DataDictionary),
			Name = nameof(Resources.DataDictionary.Title))]
		public string? Title { get; set; }
		// **********

		// **********
		/// <summary>
		/// فعال/غیر فعال بودن
		/// </summary>
		[System.ComponentModel.DataAnnotations.Display
			(Name = nameof(Resources.DataDictionary.IsActive),
			ResourceType = typeof(Resources.DataDictionary))]
		public bool IsActive { get; set; }
		// **********

		// **********
		/// <summary>
		/// توضیحات
		/// </summary>
		[System.ComponentModel.DataAnnotations.Display
			(ResourceType = typeof(Resources.DataDictionary),
			Name = nameof(Resources.DataDictionary.Description))]
		public string? Description { get; set; }
		// **********

		// **********
		/// <summary>
		/// تاریخ و زمان آخرین بروزرسانی
		/// </summary>
		[System.ComponentModel.DataAnnotations.Display
			(Name = nameof(Resources.DataDictionary.UpdateDateTime),
			ResourceType = typeof(Resources.DataDictionary))]
		public System.DateTime UpdateDateTime { get; private set; }
		// **********

		// **********
		public virtual System.Collections.Generic.IList<PostCategory> SubCategories { get; set; }
		// **********

		// **********
		//public virtual System.Collections.Generic.IList<Post> Posts { get; set; }
		// **********
		#endregion Property(ies)

		#region Method(s)
		public void SetUpdateDateTime()
		{
			UpdateDateTime = SeedWork.Utility.Now;
		}
		#endregion /Method(s)
	}
}
