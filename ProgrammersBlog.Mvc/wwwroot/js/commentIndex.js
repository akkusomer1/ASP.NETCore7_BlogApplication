$(document).ready(function () {

    /* DataTables start here. */

    const dataTable = $('#commentsTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        buttons: [            
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Comment/GetAllComments/',
                        contentType: "application/json",
                        beforeSend: function () {
                            $('#commentsTable').hide();
                            $('.spinner-border').show();
                        },
                        success: function (data) {
                            const commentResult = jQuery.parseJSON(data);
                            dataTable.clear();

                            if (commentResult.Data) {
                       
                                $.each(commentResult.Data.Comments.$values,
                                    function (index, comment) {


                                        const newTableRow = dataTable.row.add([
                                            comment.Id,
                                            comment.Article.Title,
                                            comment.Text.length > 75 ? comment.Text.substring(0, 75) : comment.Text,
                                            `${comment.IsActive ? "Evet" : "Hayır"}`,
                                            `${comment.IsDeleted ? "Evet" : "Hayır"}`,
                                            `${convertToDateString(comment.CreatedDate)}`,
                                            comment.CreatedByName,
                                            `${convertToDateString(comment.UpdateDate)}`,
                                            comment.UpdateByName,
                                            getButtonsForDataTable(comment)
                                        ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', `${comment.Id}`);
                                    });
                                dataTable.draw();
                                $('.spinner-border').hide();
                                $('#commentsTable').fadeIn(1400);
                            }
                            else {
                                toastr.error(`${commentResult.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            $('.spinner-border').hide();
                            $('#commentsTable').fadeIn(1000);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    });
                }
            }
        ],
        language: {
            "sDecimal": ",",
            "sEmptyTable": "Tabloda herhangi bir veri mevcut değil",
            "sInfo": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
            "sInfoEmpty": "Kayıt yok",
            "sInfoFiltered": "(_MAX_ kayıt içerisinden bulunan)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Sayfada _MENU_ kayıt göster",
            "sLoadingRecords": "Yükleniyor...",
            "sProcessing": "İşleniyor...",
            "sSearch": "Ara:",
            "sZeroRecords": "Eşleşen kayıt bulunamadı",
            "oPaginate": {
                "sFirst": "İlk",
                "sLast": "Son",
                "sNext": "Sonraki",
                "sPrevious": "Önceki"
            },
            "oAria": {
                "sSortAscending": ": artan sütun sıralamasını aktifleştir",
                "sSortDescending": ": azalan sütun sıralamasını aktifleştir"
            },
            "select": {
                "rows": {
                    "_": "%d kayıt seçildi",
                    "0": "",
                    "1": "1 kayıt seçildi"
                }
            }
        }
    });

    /* DataTables end here */

    /* Ajax POST / Deleting a Comment starts from here */

    $(document).on('click',
        '.btn-delete',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            let commentText = tableRow.find('td:eq(2)').text();
            commentText = commentText.length > 75 ? commentText.substring(0, 75) : commentText;
            Swal.fire({
                title: 'Silmek istediğinize emin misiniz?',
                text: `${commentText} içerikli yorum silinicektir!`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, silmek istiyorum.',
                cancelButtonText: 'Hayır, silmek istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { commentId: id },
                        url: '/Admin/Comment/Delete/',
                        success: function (data) {
                            const commentResult = jQuery.parseJSON(data);
                            console.log(commentResult)
                         
                            if (commentResult.ResultStatus===0) {
                                Swal.fire(
                                    'Silindi!',
                                    `Yorum başarıyla silinmiştir`,
                                    'success'
                                );

                                dataTable.row(tableRow).remove().draw();
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `Böyle bir yorum bulunamadı.`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!");
                        }
                    });
                }
            });
        });

    /* Ajax GET / Getting the _CommentUpdatePartial as Modal Form starts from here. */

    $(function () {
        const url = '/Admin/Comment/Update/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $(document).on('click',
            '.btn-update',
            function (event) {
                event.preventDefault();
                const id = $(this).attr('data-id');
                $.get(url, { commentId: id }).done(function (data) {
                    placeHolderDiv.html(data);
                    placeHolderDiv.find('.modal').modal('show');
                }).fail(function (err) {
                    toastr.error(`${err.responseText}`, 'Hata!');
                });
            });

        /* Ajax POST / Updating a Comment starts from here */

        placeHolderDiv.on('click',
            '#btnUpdate',
            function (event) {
                event.preventDefault();
                const form = $('#form-comment-update');
                const actionUrl = form.attr('action');
                const dataToSend = new FormData(form.get(0));
                $.ajax({
                    url: actionUrl,
                    type: 'POST',
                    data: dataToSend,
                    processData: false,
                    contentType: false,
                    success: function (data) {
                        const commentUpdateAjaxModel = jQuery.parseJSON(data);
                        console.log(commentUpdateAjaxModel);
                        //if (commentUpdateAjaxModel) {
                        //    const id = commentUpdateAjaxModel.CommentDto.Comment.Id;
                        //    const tableRow = $(`[name="${id}"]`);
                        //}
                        const id = commentUpdateAjaxModel.CommentDto.Comment.Id;
                        const tableRow = $(`[name="${id}"]`);
                        const newFormBody = $('.modal-body', commentUpdateAjaxModel.CommentUpdatePartial);
                        placeHolderDiv.find('.modal-body').replaceWith(newFormBody);
                        const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                        if (isValid) {
                            placeHolderDiv.find('.modal').modal('hide');
                            dataTable.row(tableRow).data([
                                commentUpdateAjaxModel.CommentDto.Comment.Id,
                                commentUpdateAjaxModel.CommentDto.Comment.Article.Title,
                                commentUpdateAjaxModel.CommentDto.Comment.Text.length > 75 ? commentUpdateAjaxModel.CommentDto.Comment.Text.substring(0, 75) : commentUpdateAjaxModel.CommentDto.Comment.Text,
                                `${commentUpdateAjaxModel.CommentDto.Comment.IsActive ? "Evet" : "Hayır"}`,
                                `${commentUpdateAjaxModel.CommentDto.Comment.IsDeleted ? "Evet" : "Hayır"}`,
                                `${convertToDateString(commentUpdateAjaxModel.CommentDto.Comment.CreatedDate)}`,
                                commentUpdateAjaxModel.CommentDto.Comment.CreatedByName,
                                `${convertToDateString(commentUpdateAjaxModel.CommentDto.Comment.UpdateDate)}`,
                                commentUpdateAjaxModel.CommentDto.Comment.UpdateByName,
                                getButtonsForDataTable(commentUpdateAjaxModel.CommentDto.Comment)
                            ]);
                            tableRow.attr("name", `${id}`);
                            dataTable.row(tableRow).invalidate();
                            toastr.success(`${commentUpdateAjaxModel.CommentDto.Comment.Id} no'lu yorum başarıyla güncellenmiştir`, "Başarılı İşlem!");
                        } else {
                            let summaryText = "";
                            $('#validation-summary > ul > li').each(function () {
                                let text = $(this).text();
                                summaryText = `*${text}\n`;
                            });
                            toastr.warning(summaryText);
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        toastr.error(`${err.responseText}`, 'Hata!');
                    }
                });
            });
        /* Ajax POST / Updating a Comment ends from here */
    });

    function getButtonsForDataTable(comment) {
        if (!comment.IsActive) {
            return `
                  <button class="btn btn-warning btn-sm btn-approve" data-id="${comment.Id
                }"><span class="fas fa-thumbs-up"></span></button>

                <button class="btn btn-info btn-sm btn-detail" data-id="${comment.Id
                }"><span class="fas fa-newspaper"></span></button>

                <button class="btn btn-primary btn-sm mt-1 btn-update" data-id="${comment.Id
                }"><span class="fas fa-edit"></span></button>

                <button class="btn btn-danger btn-sm mt-1 btn-delete" data-id="${comment.Id
                }"><span class="fas fa-minus-circle"></span></button>
                 `;
        }

        return `<button class="btn btn-info btn-sm btn-detail" data-id="${comment.Id}"><span class="fas fa-newspaper"></span></button>
                                <button class="btn btn-primary btn-sm mt-1 btn-update" data-id="${comment.Id}"><span class="fas fa-edit"></span></button>
                                <button class="btn btn-danger btn-sm mt-1 btn-delete" data-id="${comment.Id}"><span class="fas fa-minus-circle"></span></button>`

    }


    // Get Detail Ajax Operation Start
    $(function () {
        const url = '/Admin/Comment/GetCommentDetail';
        const placeHolderDiv = $('#modalPlaceHolder');


        $(document).on('click',
            '.btn-detail',
            function (event) {
                event.preventDefault();
                const id = $(this).attr('data-id');
                $.get(url, { commentId: id }).done(function (data) {
                    placeHolderDiv.html(data);
                    placeHolderDiv.find('.modal').modal('show');
                }).fail(function (err) {
                    toastr.error(`${err.responseText}`, 'Hata!');
                });
            });
    });
    // Get Detail Ajax Operation End


    /* Ajax POST / Approve a Comment starts from here */

    $(document).on('click',
        '.btn-approve',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            let commentText = tableRow.find('td:eq(2)').text();
            commentText = commentText.length > 75 ? commentText.substring(0, 75) : commentText;
            Swal.fire({
                title: 'Onaylamak istediğinize emin misiniz?',
                text: `${commentText} içerikli yorum onaylanacaktır!`,
                icon: 'info',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, onaylamak istiyorum.',
                cancelButtonText: 'Hayır, onaylamak istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { commentId: id },
                        url: '/Admin/Comment/Approve/',
                        success: function (data) {
                            const commentResult = jQuery.parseJSON(data);                      
                            if (commentResult.Data) {
                                console.log("onayla!!!")
                                console.log(commentResult);
                                dataTable.row(tableRow).data([
                                    commentResult.Data.Comment.Id,
                                    commentResult.Data.Comment.Article.Title,
                                    commentResult.Data.Comment.Text.length > 75 ? commentResult.Comment.Text.substring(0, 75) : commentResult.Data.Comment.Text,
                                    `${commentResult.Data.Comment.IsActive ? "Evet" : "Hayır"}`,
                                    `${commentResult.Data.Comment.IsDeleted ? "Evet" : "Hayır"}`,
                                    `${convertToDateString(commentResult.Data.Comment.CreatedDate)}`,
                                    commentResult.Data.Comment.CreatedByName,
                                    `${convertToDateString(commentResult.Data.Comment.UpdateDate)}`,
                                    commentResult.Data.Comment.UpdateByName,
                                    getButtonsForDataTable(commentResult.Data.Comment)
                                ]);
                                tableRow.attr("name", `${id}`);
                                dataTable.row(tableRow).invalidate();


                                Swal.fire(
                                    'Onaylandı!',
                                    `${commentResult.Data.Comment.Id} no'lu yorum başarıyla onaylanmıştır.`,
                                    'success'
                                );

                              
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `Böyle bir yorum bulunamadı.`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!");
                        }
                    });
                }
            });
        });

   



});
