$(document).ready(function () {
    // Khi click vào danh mục
    $('.menu-category a').on('click', function (e) {
        e.preventDefault();
        var categoryId = $(this).data('category-id');

        // Gửi yêu cầu AJAX đến server
        $.ajax({
            url: '@Url.Action("GetProductsByCategory", "ProductView")', // URL của action trên Controller
            data: { categoryId: categoryId },
            type: 'GET',
            success: function (data) {
                // Cập nhật danh sách sản phẩm
                $('#product-list-container .recommend-section').html(data);
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải sản phẩm.');
            }
        });
    });
});
