# Work Item List 專案說明

## 專案簡介

**Work Item List** 是一個工作清單管理系統，
提供 **列表** 與 **詳情** 頁面，讓使用者可以方便地：

* 建立、瀏覽、修改與刪除工作項目（CRUD）
* 新增項目時需填寫：

  * **標題 (Title)**：必填欄位
  * **說明 (Description)**：選填欄位
* 系統支援 **Swagger UI** 可視化展示 API 參數與呼叫方式（HTTP Method），並可直接在頁面上進行測試。
* 提供 **NUnit 單元測試**，確保主要功能的正確性與穩定性

---

## 系統架構

本專案採用 **ASP.NET Core MVC** 並實作 **三層式架構**：

| 層級                        | 功能說明                                           |
| ------------------------- | ---------------------------------------------- |
| **Controller**            | 負責接收使用者的 **HTTP 請求**，根據情境決定回應方式：<br>－ 若為網頁操作，則回傳 **MVC View 頁面**<br>－ 若為前端 AJAX 呼叫，則以 **RESTful API** 格式回傳 **HTTP 狀態碼**            |
| **Service**               | 負責處理商業邏輯                                           |
| **Repository**            | 使用 Dapper 與 MSSQL 互動（透過 Stored Procedure 存取資料） |

---

## 技術

| 類別            | 使用技術                                 |
| ------------- | ------------------------------------ |
| **後端框架**      | ASP.NET Core MVC                  |
| **資料存取**      | Dapper ORM + MSSQL（Stored Procedure） |
| **前端**        | Razor View + jQuery + Ajax           |
| **API 文件**    | Swagger (Swashbuckle.AspNetCore)     |
| **依賴注入 (DI)** |  .NET Core DI 容器                   |
| **單元測試**      | NUnit + Moq                          |

---
