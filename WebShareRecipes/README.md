# Dự án ShareRecipes - ASP.NET
Đây là dự án cho môn học ASP.NET, được thực hiện bởi Quang Thái. Dự án này là một ứng dụng web chia sẻ công thức nấu ăn (ShareRecipes). Người dùng có thể xem, thêm và chia sẻ các công thức nấu ăn.

## Mục tiêu
- Xây dựng một ứng dụng web sử dụng ASP.NET Web Application.
- Tích hợp cơ sở dữ liệu để lưu trữ công thức.
- Cung cấp giao diện thân thiện để người dùng tương tác.

## Tiến độ thực hiện (cập nhật liên tục)
**Ngày 12/04/2025**: Khởi tạo dự án, thêm các thư mục cơ bản (Controllers, Models, Views), và tạo file README.md.

**Ngày 15/04/2025**: 
- Thiết lập cơ sở dữ liệu với Entity Framework. Tạo các model cơ bản: `Recipe`, `RecipeStep`, `Category`, và `User`.
- Thêm file `ApplicationDbContext.cs` để quản lý kết nối cơ sở dữ liệu.
- Tạo `RecipesController` với các action cơ bản: `Create`, `Edit`, `Delete`, và `MyRecipes`.

**Ngày 20/04/2025**: 
- Tạo giao diện cơ bản với Razor Views: `Create.cshtml`, `Edit.cshtml`, và `MyRecipes.cshtml`.
- Thêm chức năng upload hình ảnh cho công thức và các bước (`RecipeSteps`) trong `Create.cshtml` và `Edit.cshtml`.
- Tích hợp client-side validation bằng jQuery Unobtrusive Validation.

**Ngày 25/04/2025**: 
- Gặp lỗi validation khi tạo công thức: tiêu đề (`Recipe.Title`) bị giới hạn 200 ký tự trong client-side, nhưng model chỉ cho phép 100 ký tự.
- Sửa lỗi bằng cách đồng bộ validation client-side và server-side: xóa `data-val-maxlength` trong `Create.cshtml` và `Edit.cshtml`, để validation dựa hoàn toàn vào model (`[StringLength(100)]` trong `RecipeViewModel`).
- Cập nhật thông báo lỗi tiếng Việt trong các file `.cshtml` (ví dụ: "Tiêu đề không được dài quá 100 ký tự").

**Ngày 30/04/2025**: 
- Yêu cầu lấy lại phiên bản code trước khi thêm validation giới hạn 200 ký tự cho `RecipeSteps.Title`.
- Cung cấp lại phiên bản cũ của `Create.cshtml` và `Edit.cshtml` (không có `data-val-maxlength` cho `RecipeSteps.Title`), nhưng vẫn giữ validation server-side dựa trên model (`[StringLength(200)]`).

**Ngày 05/05/2025**: 
- Phát hiện lỗi khi chỉnh sửa công thức: nếu không chọn hình ảnh mới cho `RecipeStep` và nhấn lưu, hệ thống báo lỗi "Vui lòng tải lên hình ảnh cho các mục chưa có hình!".
- Nguyên nhân: Logic trong hàm `validateForm()` trong `Edit.cshtml` không cho phép giữ hình ảnh cũ (`ImageUrl` cũ) nếu không chọn file mới.
- Sửa lỗi bằng cách cải tiến hàm `validateForm()`: chỉ yêu cầu hình ảnh mới nếu không có hình ảnh cũ (`hasStepImage` là `false`). Nếu đã có `ImageUrl` cũ, không bắt buộc tải file mới.
- Cập nhật `Edit.cshtml` để đảm bảo logic validation linh hoạt hơn, giữ nguyên hình ảnh cũ khi không có file mới.

**Ngày 06/05/2025**: 
- Tạo file `README.md` mô tả lại toàn bộ tiến độ thực hiện dự án theo dạng nhật ký.
- Tiếp tục kiểm tra và tối ưu giao diện, chuẩn bị báo cáo cuối kỳ.



# Dự án ShareRecipes - ASP.NET

Đây là dự án cho môn học ASP.NET, được thực hiện bởi Quang Thái. Dự án này là một ứng dụng web chia sẻ công thức nấu ăn (ShareRecipes). Người dùng có thể xem, thêm và chia sẻ các công thức nấu ăn.

## Mục tiêu

* Xây dựng một ứng dụng web sử dụng ASP.NET Web Application.
* Tích hợp cơ sở dữ liệu để lưu trữ công thức.
* Cung cấp giao diện thân thiện để người dùng tương tác.

## Tiến độ thực hiện (cập nhật liên tục)

**Ngày 12/04/2025**: Khởi tạo dự án, thêm các thư mục cơ bản (Controllers, Models, Views), và tạo file README.md.

**Ngày 15/04/2025**:

* Thiết lập cơ sở dữ liệu với Entity Framework. Tạo các model cơ bản: `Recipe`, `RecipeStep`, `Category`, và `User`.
* Thêm file `ApplicationDbContext.cs` để quản lý kết nối cơ sở dữ liệu.
* Tạo `RecipesController` với các action cơ bản: `Create`, `Edit`, `Delete`, và `MyRecipes`.

**Ngày 20/04/2025**:

* Tạo giao diện cơ bản với Razor Views: `Create.cshtml`, `Edit.cshtml`, và `MyRecipes.cshtml`.
* Thêm chức năng upload hình ảnh cho công thức và các bước (`RecipeSteps`) trong `Create.cshtml` và `Edit.cshtml`.
* Tích hợp client-side validation bằng jQuery Unobtrusive Validation.

**Ngày 25/04/2025**:

* Gặp lỗi validation khi tạo công thức: tiêu đề (`Recipe.Title`) bị giới hạn 200 ký tự trong client-side, nhưng model chỉ cho phép 100 ký tự.
* Sửa lỗi bằng cách đồng bộ validation client-side và server-side: xóa `data-val-maxlength` trong `Create.cshtml` và `Edit.cshtml`, để validation dựa hoàn toàn vào model (`[StringLength(100)]` trong `RecipeViewModel`).
* Cập nhật thông báo lỗi tiếng Việt trong các file `.cshtml` (ví dụ: "Tiêu đề không được dài quá 100 ký tự").

**Ngày 30/04/2025**:

* Yêu cầu lấy lại phiên bản code trước khi thêm validation giới hạn 200 ký tự cho `RecipeSteps.Title`.
* Cung cấp lại phiên bản cũ của `Create.cshtml` và `Edit.cshtml` (không có `data-val-maxlength` cho `RecipeSteps.Title`), nhưng vẫn giữ validation server-side dựa trên model (`[StringLength(200)]`).

**Ngày 05/05/2025**:

* Phát hiện lỗi khi chỉnh sửa công thức: nếu không chọn hình ảnh mới cho `RecipeStep` và nhấn lưu, hệ thống báo lỗi "Vui lòng tải lên hình ảnh cho các mục chưa có hình!".
* Nguyên nhân: Logic trong hàm `validateForm()` trong `Edit.cshtml` không cho phép giữ hình ảnh cũ (`ImageUrl` cũ) nếu không chọn file mới.
* Sửa lỗi bằng cách cải tiến hàm `validateForm()`: chỉ yêu cầu hình ảnh mới nếu không có hình ảnh cũ (`hasStepImage` là `false`). Nếu đã có `ImageUrl` cũ, không bắt buộc tải file mới.
* Cập nhật `Edit.cshtml` để đảm bảo logic validation linh hoạt hơn, giữ nguyên hình ảnh cũ khi không có file mới.

**Ngày 06/05/2025**:

* Tạo file `README.md` mô tả lại toàn bộ tiến độ thực hiện dự án theo dạng nhật ký.
* Tiếp tục kiểm tra và tối ưu giao diện, chuẩn bị báo cáo cuối kỳ.

## Hướng dẫn Sử dụng

### 1. Cài đặt Môi trường Phát triển

* **Yêu cầu phần mềm:**

  * Visual Studio 2022 (hoặc mới hơn)
  * SQL Server / SQL Server Express
  * .NET SDK (.NET 6.0 hoặc .NET 7.0)
  * Entity Framework Core Tools (cài qua NuGet hoặc CLI)

* **Các bước cài đặt đề xuất:**

  * Cài đặt Visual Studio với workload **ASP.NET and web development**.
  * Cài SQL Server và SSMS (SQL Server Management Studio) để quản lý cơ sở dữ liệu.

### 2. Clone hoặc Tải Xuống Dự Án

```bash
git clone https://github.com/QuangThaiii/ASPNET-DK23TTC11-lenguyenquangthai-WebShareRecipes.git
```

Hoặc tải file `.zip` từ GitHub, sau đó giải nén.

### 3. Thiết lập Cơ sở Dữ liệu

* Mở file `appsettings.json`, sửa chuỗi kết nối `DefaultConnection` theo cấu hình máy bạn. Ví dụ:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=WebShareRecipes;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

> ⚠️ Đảm bảo rằng SQL Server đang chạy và có quyền tạo database.

### 4. Chạy Migrations và Khởi tạo CSDL

* Mở **Package Manager Console** (PMC) trong Visual Studio.
* Chạy lần lượt các lệnh sau:

```powershell
Add-Migration InitialCreate
Update-Database
```

> 🛠 Nếu migration đã được tạo, chỉ cần chạy `Update-Database`.

### 5. Chạy Ứng dụng

* Nhấn `F5` hoặc chọn **Debug > Start Debugging** trong Visual Studio.
* Trình duyệt sẽ mở trang chủ của ứng dụng.

### 6. Đăng ký và Đăng nhập

* Click vào nút **Đăng ký** trên navbar.
* Điền thông tin để tạo tài khoản.
* Sau đó đăng nhập để sử dụng đầy đủ chức năng.
* Mặc định có tài khoản admin với thông tin đăng nhập như sau:

  * **Username:** `myAdmin`
  * **Password:** `my@dmin`
* Điền thông tin để tạo tài khoản.
* Sau đó đăng nhập để sử dụng đầy đủ chức năng.

### 7. Cập nhật và Sử dụng Ứng dụng

* **Với tài khoản admin**, bạn có thể sử dụng các chức năng quản trị:

  * **Quản lý chủ đề**: thêm, sửa, xóa các danh mục chủ đề.
  * **Quản lý bài viết**: duyệt bài mới, xóa bài, xem chi tiết từng bài viết.
  * **Quản lý người dùng**: cập nhật thông tin người dùng, xóa tài khoản khi cần thiết.

* **Trang chủ**: Xem danh sách công thức mới.

* **Thêm bài viết**: Vào mục "Bài viết của tôi" nhấn nút "Tạo bài viết mới" để tạo bài viết mới (bao gồm tiêu đề, mô tả, hình ảnh và các bước thực hiện).

* **Chỉnh sửa/Xóa**: Quản lý bài viết đã đăng qua trang "Bài viết của tôi".

* **Phân loại**: Lọc công thức theo danh mục.

### 8. Báo cáo Cuối kỳ

* Tổng hợp tiến độ, ảnh chụp màn hình, và các lỗi đã xử lý.
* Viết tài liệu báo cáo theo yêu cầu môn học.
* Đóng gói source code, backup CSDL nếu cần nộp.
