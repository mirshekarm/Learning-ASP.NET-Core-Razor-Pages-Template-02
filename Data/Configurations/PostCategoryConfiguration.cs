using Microsoft.EntityFrameworkCore;

namespace Data.Configurations
{
	internal class PostCategoryConfiguration :
		object, Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Domain.PostCategory>
	{
		public PostCategoryConfiguration() : base()
		{
		}


		public void Configure
			(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.PostCategory> builder)
		{
			// **************************************************
			builder
				.Property(current => current.Title)
				.HasMaxLength(Constants.MaxLength.Title)
				.IsRequired(required: true)
				.IsUnicode(true)
				;
			// **************************************************

			// **************************************************
			builder
				.Property(current => current.Description)
				.HasMaxLength(maxLength: Domain.PostCategory.DescriptionMaxLength)
				.IsRequired(required: false)
				.IsUnicode(unicode: true)
				;
			// **************************************************

			// **************************************************
			builder
				.HasMany(current => current.SubCategories)
				.WithOne(other => other.Parent)
				.IsRequired(required: false)
				.HasForeignKey(other => other.ParentId)
				.OnDelete(deleteBehavior: Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction)
				;
			// **************************************************

			//**************************************************
			builder
				.HasIndex(current => new { current.ParentId, current.Title })
				.IsUnique(unique: true)
				;
			//**************************************************
		}
	}
}
