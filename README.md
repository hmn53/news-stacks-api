# News Stack API

This is an API built with .NET Core for a news website. The API has three user roles: Writer, Publisher, and Reader. Writers can create, update, and delete articles and submit them for publishing. Publishers can add SEO metadata, tags, and publish articles. Readers can view all published articles and filter them based on tags.

## Table of Contents
- [User Roles and Routes](#user-roles-and-routes)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## User Roles and Routes

### Writer
Writers can create, update, and delete articles and submit them for publishing.

#### Routes
- `GET /api/writer/articles/{articleId}` - Get a single article from the given article ID.
- `GET /api/writer/articles/all` - Get all articles.
- `GET /api/writer/articles/assigned` - Get all articles assigned to the current writer.
- `POST /api/writer/article` - Create a new article. The current writer will be assigned to this article.
- `PUT /api/writer/article/{articleId}` - Update an article. The current writer will be assigned to this article.
- `DELETE /api/writer/article/{articleId}` - Delete an article. The current writer can only delete an article if it is assigned to him/her.
- `POST /api/writer/article/submit/{articleId}` - Submit an article for publishing. The current writer can only submit an article if it is assigned to him/her.

### Publisher
Publishers can add SEO metadata, tags, and publish articles.

#### Routes
- `GET /api/publisher/articles/{articleId}` - Get a single article from the given article ID.
- `GET /api/publisher/articles/all` - Get all submitted articles.
- `GET /api/publisher/articles/assigned` - Get all articles assigned to the current publisher.
- `POST /api/publisher/article/seo/{articleId}` - Add metadata for SEO to the given article.
- `POST /api/publisher/article/tags/{articleId}` - Add tags to the given article.
- `POST /api/publisher/article/publish/{articleId}` - Publish the given article.

### Reader
Readers can view all published articles and filter them based on tags.

#### Routes
- `GET /api/reader/articles/all` - Get all published articles.
- `GET /api/reader/articles/{articleId}` - Get a single published article.
- `GET /api/reader/articles/{tag}` - Get articles based on tag.

## Installation
1. Clone the repository to your local machine using `git clone https://github.com/hmn53/news-stacks-api`
2. Navigate to the project directory and run `dotnet build` to build the project.
3. Run the database migrations by running `dotnet ef database update`.

## Usage
1. Start the development server by running `dotnet run`.
2. Navigate to `http://localhost:5000` in your web browser or use a tool like Postman to test the API.
3. Register a new user account as a writer, publisher, or reader.
4. Log in as a writer or publisher to create, update, and submit articles or add SEO metadata and tags.
5. Log in as a reader to view all published articles or filter them based on tags.

## Contributing
If you find any bugs or have suggestions for improvement, please feel free to open an issue or submit a pull request.
