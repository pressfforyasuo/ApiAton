<h1>WebApi</h1>
<h2>Этот проект использует три пакета:</h2>

<li>Microsoft.EntityFrameworkCore</li>
<li>Microsoft.EntityFrameworkCore.SqlServer</li>
<li>Swashbuckle.AspNetCore</li>

<h2>Для корректной работы необходимо создать и установить связь с базой данных</h2>
<li>В обозревателе серверов необходимо создать новую базу данных(Указав имя сервера: localhost\* и имя базы данный: User)</li>
<li>Установим подключение к базе данных в файле UserContext.cs</li>

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=User;TrustServerCertificate=True;Integrated Security=True;Pooling=True");
}

```

<li>Создадим sql запрос в котором содержи</li>

```sql
CREATE TABLE Users (
    Guid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Login VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(50) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Gender INT NOT NULL DEFAULT 2,
    Birthday DATETIME,
    Admin BIT NOT NULL,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(50) NOT NULL,
    ModifiedOn DATETIME,
    ModifiedBy VARCHAR(50),
    RevokedOn DATETIME,
    RevokedBy VARCHAR(50)
);

INSERT INTO Users (Guid, Login, Password, Name, Gender, Birthday, Admin, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
VALUES (NEWID(), 'admin', 'adminpassword', 'Admin', 1, NULL, 1, GETDATE(), 'system', GETDATE(), 'system');

```

<h2>Структура проекта:</h2>
<h5>Controllers/UsersController.cs - содержит все запросы указанные в ТЗ</h5>
<h5>Models/User.cs - содержит описание сущности user</h5>
<h5>Models/UserContext.cs - содержит методы для подключения к бд</h5>
<h5>Services/Helper.cs - содержит вспомогательные методы</h5>

<h2>Описание запросов</h2>
<h5>GetAllActiveUser - принимает на вход логин и пароль для аутентификации пользователя, если пользователь является админом, то возвращает всех активных пользователей отсортированных по полю CreatedOn.</h5>
<h5>GetUser - принимает на вход логин, пароль и логин по которому производится поиск, если пользователь является админом, то происходит проверка наличия данного логина в базе данных. Если логин существует, выводится список с полями имя, пол, дата рождения, статус активный или нет.</h5>
<h5>GetMe - принимает на вход логин и пароль, если пользователь является активным, то возвращает список с его данными.</h5>
<h5>GetUsersByAge - принимает на вход логин, пароль и возраст, если пользователь является админом, то возвращает список с пользователями у которых возраст больше указанного.</h5>
<h5>CreteUser - принимает на вход логин и пароль, а так же все необходимые поля соответствующие сущности user для создания нового пользователя. Если пользователь явлеяется админом, то он может создать нового пользователя.</h5>
<h5>UpdatePersonalData - принимает на вход логин и пароль, а так же новые значения для полей которые необходимо заменить. Если пользователь является админом, то он может заменить данные других пользователей</h5>
<h5>UpdatePassword - принимает на вход логин, пароль и новый пароль, если пользователь является админом, то может поменять пароль у другого пользователя</h5>
<h5>UpdateLogin - принимает на вход логин, пароль и новый логин, если пользователь является админом, то может поменять логин у другого пользователя</h5>
<h5>RestoreUser - принимает на вход логин, пароль и логин пользователя, которого необходимо востановить(доступно админу)</h5>
<h5>DeleteUser - принимается на вход логин, пароль, логин пользователя, которого необходимо удалить и значение по которому определяется будет удаление жестким или мягким(доступно админу)</h5>
