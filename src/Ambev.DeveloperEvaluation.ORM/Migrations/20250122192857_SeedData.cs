using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO public.""Users"" (""Id"", ""Username"", ""Email"", ""Phone"", ""Password"", ""Role"", ""Status"", ""CreatedAt"", ""IsActive"")
            VALUES (
                '24ac6b75-471b-4f17-96ff-7d0e7510461d', 
                'henrique', 
                'henrique.andradesilva@hotmail.com', 
                '11 912345678', 
                '$2a$11$VJi1U9BHfvCFtS8RVeBFQu.PeBjUzm7.kgmgzw9Dnguwxp.sREGna', 
                3,
                1,
                NOW(),
                true)");

            migrationBuilder.Sql(@"
            INSERT INTO public.""Sales"" (""Id"", ""Number"", ""Customer"", ""Branch"", ""TotalAmount"", ""Date"", ""IsCancelled"", ""CreatedAt"", ""IsActive"")
            VALUES 
            ('11111111-1111-1111-1111-111111111111', 'AASQWUEIQWE', 'Customer A', 'Branch X', 280, NOW(), false, NOW(), true),
            ('22222222-2222-2222-2222-222222222222', 'ASDKASDJADS', 'Customer B', 'Branch Y', 356, NOW(), false, NOW(), true),
            ('33333333-3333-3333-3333-333333333333', 'LKAASDAJSDL', 'Customer C', 'Branch Z', 642, NOW(), false, NOW(), true)");

            migrationBuilder.Sql(@"
            INSERT INTO public.""SaleItems"" (""Id"", ""SaleId"", ""Product"", ""Quantity"", ""UnitPrice"", ""Discount"", ""TotalAmount"", ""IsCancelled"", ""CreatedAt"", ""IsActive"")
            VALUES 
            ('11111111-aaaa-aaaa-aaaa-111111111111', '11111111-1111-1111-1111-111111111111', 'Product A1', 10, 20.5, 0, 205, false, NOW(), true),
            ('11111111-bbbb-bbbb-bbbb-111111111111', '11111111-1111-1111-1111-111111111111', 'Product A2', 5, 15.0, 0, 75, false, NOW(), true),
            ('22222222-cccc-cccc-cccc-222222222222', '22222222-2222-2222-2222-222222222222', 'Product B1', 8, 30.0, 0, 240, false, NOW(), true),
            ('22222222-dddd-dddd-dddd-222222222222', '22222222-2222-2222-2222-222222222222', 'Product B2', 12, 10.0, 2, 116, false, NOW(), true),
            ('33333333-aaaa-aaaa-aaaa-333333333333', '33333333-3333-3333-3333-333333333333', 'Product C1', 5, 50.0, 5, 245, false, NOW(), true),
            ('33333333-bbbb-bbbb-bbbb-333333333333', '33333333-3333-3333-3333-333333333333', 'Product C2', 15, 20.0, 3, 297, false, NOW(), true)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DELETE FROM public.""Users""
            WHERE ""Id = '24ac6b75-471b-4f17-96ff-7d0e7510461d'");

            // Remove os SaleItems inseridos
            migrationBuilder.Sql(@"
            DELETE FROM public.""SaleItems""
            WHERE ""SaleId"" IN ('11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222')");

            // Remove os Sales inseridos
            migrationBuilder.Sql(@"
            DELETE FROM public.""Sales""
            WHERE ""Id"" IN ('11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222')");
        }
    }
}
