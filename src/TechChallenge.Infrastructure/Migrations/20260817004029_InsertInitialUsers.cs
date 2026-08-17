using Microsoft.EntityFrameworkCore.Migrations;

namespace TechChallenge.Infrastructure.Migrations;

public partial class InsertInitialUsers : Migration
{

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            SET IDENTITY_INSERT Users ON;

            INSERT INTO Users (Id, Name, Email, Login, Password, Perfil, CreatedAt, UpdatedAt) VALUES
            (1, 'Pedro', 'pedroadmin@teste.com', 'pedroAdm', '123456', 1, GETDATE(), GETDATE()),
            (2, 'Julia', 'juliaadmin@teste.com', 'juliaadmin', '123456', 1, GETDATE(), GETDATE()),
            (3, 'Paulo', 'paulo@teste.com', 'ph123', '123456', 0, GETDATE(), GETDATE()),
            (4, 'Mara', 'mara@teste.com', 'marahjk', '123456', 0, GETDATE(), GETDATE()),
            (5, 'Felipe', 'felipepq@teste.com', 'lipeBo', '123456', 0, GETDATE(), GETDATE()),
            (6, 'Clara', 'clarinhadg@teste.com', 'clarinhali', '123456', 0, GETDATE(), GETDATE()),
            (7, 'Tiago', 'tiago-santos@teste.com', 'santos-ti', '123456', 0, GETDATE(), GETDATE()),
            (8, 'Marina', 'marina_souza@teste.com', 'marimari', '123456', 0, GETDATE(), GETDATE()),
            (9, 'Theo', 'theopastel@teste.com', 'pastel22', '123456', 0, GETDATE(), GETDATE()),
            (10, 'Ana', 'ana_banana@teste.com', 'banana_ana', '123456', 0, GETDATE(), GETDATE());

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