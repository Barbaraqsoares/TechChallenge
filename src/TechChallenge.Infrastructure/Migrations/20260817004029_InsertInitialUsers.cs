using Microsoft.EntityFrameworkCore.Migrations;

namespace TechChallenge.Infrastructure.Migrations;

public partial class InsertInitialUsers : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            SET IDENTITY_INSERT Users ON;

            INSERT INTO Users (Id, Name, Email, Login, PasswordHash, Perfil, CreatedAt, UpdatedAt) VALUES
            (1, 'Pedro', 'pedroadmin@teste.com', 'pedroAdm', '', 1, GETDATE(), GETDATE()),
            (2, 'Julia', 'juliaadmin@teste.com', 'juliaadmin', '', 1, GETDATE(), GETDATE()),
            (3, 'Paulo', 'paulo@teste.com', 'ph123', '', 0, GETDATE(), GETDATE()),
            (4, 'Mara', 'mara@teste.com', 'marahjk', '', 0, GETDATE(), GETDATE()),
            (5, 'Felipe', 'felipepq@teste.com', 'lipeBo', '', 0, GETDATE(), GETDATE()),
            (6, 'Clara', 'clarinhadg@teste.com', 'clarinhali', '', 0, GETDATE(), GETDATE()),
            (7, 'Tiago', 'tiago-santos@teste.com', 'santos-ti', '', 0, GETDATE(), GETDATE()),
            (8, 'Marina', 'marina_souza@teste.com', 'marimari', '', 0, GETDATE(), GETDATE()),
            (9, 'Theo', 'theopastel@teste.com', 'pastel22', '', 0, GETDATE(), GETDATE()),
            (10, 'Ana', 'ana_banana@teste.com', 'banana_ana', '', 0, GETDATE(), GETDATE());

            SET IDENTITY_INSERT Users OFF;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        for (int id = 1; id <= 10; id++)
        {
            migrationBuilder.Sql($"DELETE FROM Users WHERE Id = {id}");
        }
    }
}