
# Blog Application

This is a simple CRUD blog application developed using **.NET 9** and **MS SQL Server**. The app supports two user roles: **Admin** and **Editor**. The application allows users to create, manage, and display blog posts with various features such as rich text editing, image uploads, and status management (Draft, Published, Rejected).

## Features

- **User Roles:**
  - **Admin**: Create and delete users, approve/reject blog posts, create/edit blog posts.
  - **Editor**: Create and edit their own blog posts.
  
- **Blog Post Management:**
  - Blog posts have the following fields:
    - **Title** (Textbox)
    - **Banner Image** (File input, restricted to JPG, PNG, GIF formats, and max size 5MB)
    - **Content** (Rich text WYSIWYG editor)

- **Post Statuses:**
  - **Draft**: Posts that are not yet published.
  - **Published**: Posts that are live and visible to all users.
  - **Rejected**: Posts that are rejected by an admin.

- **Admin Features:**
  - Approve or reject blog posts.
  - Manage users (create/delete).
  
- **Editor Features:**
  - Create and edit their own blog posts.
  
- **Pages:**
  - **Drafts**: View and manage blog posts in draft status (can be published or rejected).
  - **Published**: View blog posts that are published. Posts can be reverted to drafts.
  - **Rejected**: View rejected posts, with an option to revert them back to drafts.
  - **Published Feed**: Display a feed of all published blog posts.

- **File Upload**: Allows users to upload banner images for their blog posts (only JPG, PNG, GIF allowed, up to 5MB in size).

- **User-Friendly Frontend**: Built with **Bootstrap 4.0**, providing a clean, responsive design for both desktop and mobile devices.

## Prerequisites

Before you can run the application, ensure the following are installed:

1. **.NET 9 SDK**: You can download it from [here](https://dotnet.microsoft.com/download).
2. **MS SQL Server**: Ensure you have MS SQL Server installed. You can use SQL Server Express for local development.

## Setup Instructions

### 1. Clone the Repository

First, clone the repository to your local machine:

```bash
git clone https://github.com/lankeshhalangoda/BlogApplication.git
```

### 2. Open the Project in Visual Studio

- Open the **BlogApplication** folder in **Visual Studio**.

### 3. Update Connection String

- Open the `appsettings.json` file and update the **DefaultConnection** with your SQL Server connection string:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BlogApp;User Id=your-username;Password=your-password;"
}
```

### 4. Apply Database Migrations

To set up the database, run the migrations:

- Open **Package Manager Console** in Visual Studio.
- Run the following command:

```powershell
Update-Database
```

This will create the necessary tables in your SQL Server database.

### 5. Run the Application

- Press **F5** or **Ctrl+F5** to run the application (or click the https 'play' button).
- The application will be available at `http://localhost:5000` by default.

## Folder Structure

- `Controllers/`: Contains controllers for handling user roles, blog posts, etc.
- `Models/`: Contains models for users, blog posts, and their statuses.
- `Views/`: Contains Razor Views for Admin and Editor panels.
- `wwwroot/`: Contains static files like images, CSS, JavaScript, and the **Instructions** folder where the demo video is stored.

## Demo Video

You can find the demo video showing the application's functionality in the **`wwwroot/Instructions`** folder as **Demo Video.mp4**.

## Contributions

Feel free to fork this repository and submit pull requests. If you encounter any issues or have feature requests, please open an issue in the Issues section.

## License

This project is licensed under the MIT License.
