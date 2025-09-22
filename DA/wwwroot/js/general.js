function GetDate() {
    var gunler = ["Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi"];
    var aylar = ["", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"];
    // Bugünün tarihini almak için yeni bir Date nesnesi oluşturun
    var bugun = new Date();

    // Tarih bilgisini alarak gün, ay ve yıl bilgilerini alın
    var gun = bugun.getDate();
    var ay = aylar[bugun.getMonth() + 1]; // JavaScript'te aylar 0'dan başlar, bu yüzden 1 eklememiz gerekir.
    var gunAdi = gunler[bugun.getDay()];

    $("#calendar").html("<i class='mdi mdi-calendar icon-sm text-primary'></i>" + gun + ' ' + ay + ' ' + gunAdi);
}
function showPreloader() {
    $('#preloader').show();
}

// AJAX isteği tamamlandığında preloader'ı gizleyen fonksiyon
function hidePreloader() {
    $('#preloader').hide();
}

// AJAX isteklerini dinleyen global AJAX olaylarını tanımlayın
$(document).ajaxStart(function () {
    showPreloader(); // AJAX isteği başladığında preloader'ı göster
});

$(document).ajaxStop(function () {
    hidePreloader(); // AJAX isteği tamamlandığında preloader'ı gizle
});

function ShowErrorMessage(message) {
    ToastrAlert(message, "Hata!", "Error");
}

function ShowWarningMessage(message) {
    ToastrAlert(message, "Uyarı!", "Info");
}

function ShowSuccessMessage(message) {
    ToastrAlert(message, "Başarılı!", "Success");
}


var $toastlast;
toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

function ToastrAlert(message, title, type) {

    if (type == "Error")
        toastr["error"](message, title)
    else if (type == "Success")
        toastr["success"](message, title)
    else if (type == "Info")
        toastr["info"](message, title)
    else if (type == "Warning")
        toastr["warning"](message, title)
    else if (type == "Alert") {
        $.alert({
            title: message,
            content: title,
        });
    }
}


function FileAjaxMethod(controllerName, formData) {

    $.ajax({
        type: "POST",
        url: "/" + controllerName + "/FileReadyToUpload",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            eval(response);
        },
        error: function (xhr, status, error) {
            eval(response);
        }
    });
}


function DoButtonOperationSil(controllerName, id) { //Silme metodu

    var myObj = {};

    myObj["Operation"] = "Delete";
    myObj["DataId"] = id;

    Swal.fire({
        title: 'Silme Onayı!',
        text: "Seçili Kayıt Silinecek. Devam Edilsin Mi?",
        icon: 'question',
        iconColor: '#ea5455',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'Vazgeç',
        customClass: {
            confirmButton: 'btn btn-primary me-3',
            cancelButton: 'btn btn-label-secondary'
        },
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            AjaxMethod(controllerName, myObj)
        }
    });
}

$(document).ready(function () {
    // Form submit işlemi
    $('form').submit(function (event) {
        event.preventDefault(); // Sayfanın yenilenmesini engelle

        // Form verilerini al
        var formData = {};
        $(this).find('input').each(function () {
            if ($(this).attr('type') === 'radio') {
                if ($(this).is(':checked')) {
                    formData[$(this).attr('name')] = $(this).val();
                }
            }
            else if ($(this).attr('type') === 'checkbox') {
                if ($(this).is(':checked')) {
                    formData[$(this).attr('name')] = "true";
                }
                else {
                    formData[$(this).attr('name')] = "false";
                }
            }
            else {
                formData[$(this).attr('name')] = $(this).val();
            }
        });

        $(this).find('textarea').each(function () {
            formData[$(this).attr('name')] = $(this).val();
        });

        $(this).find('select').each(function () {
            formData[$(this).attr('name')] = $(this).val();
        });

        // AJAX isteği gönder
        $.ajax({
            type: "POST",
            url: $(this).attr('action'), // Formun action attribute'undan URL'yi al
            data: formData,
            success: function (response) {
                eval(response)
            },
            error: function (xhr, status, error) {
                ShowErrorMessage('Bir hata oluştu. Sistem yöneticisi ile görüşün.');
            }
        });
    });
});

function AjaxMethod(url, value, method) {
    event.preventDefault();

    if (method == "Delete") {
        Swal.fire({
            title: 'Silme Onayı!',
            text: "Seçili Kayıt Silinecek. Devam Edilsin Mi?",
            icon: 'question',
            iconColor: '#ea5455',
            showCancelButton: true,
            confirmButtonText: 'Evet, sil!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "AcceptMission") {
        Swal.fire({
            title: 'Kabul Onayı!',
            text: "Seçili görev kabul edilecek. İşlemin geri dönüşü olmaz. Devam Edilsin Mi?",
            icon: 'question',
            iconColor: '#008000',
            showCancelButton: true,
            confirmButtonText: 'Evet, kabul et!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "RejectMission") {
        Swal.fire({
            title: 'Reddetme Onayı!',
            inputAttributes: {
                autocapitalize: "off"
            },
            text: "Seçili görev reddedilecek. Devam edilsin mi?",
            icon: 'question',
            iconColor: '#ea5455',
            showCancelButton: true,
            confirmButtonText: 'Evet, reddet!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            //var reason = $('.swal-content__textarea').val();
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "AcceptPermission") {
        Swal.fire({
            title: 'Kabul Onayı!',
            html: `<p>Seçili izin kabul edilecek. İşlemin geri dönüşü olmaz. Eğer izni kabul ediyorsanız iznin tipini aşağıdan seçiniz. İzin oluşturulurken "Yıllık İzin" olarak oluşturulmuştur. (Seçtiğiniz izin tipine göre kişinin izin sayısından düşüş yapılacaktır.)</p>
           <div class="row mb-2">
                <label for="" class="col-sm-2 col-form-label pt-0">İzin Türü</label>
                <div class="col-sm-10">
                    <div class="form-group row">
                        <div class="col-sm-6">
                            <div class="form-check">
                                <label class="form-check-label">
                                    <input type="radio" class="form-check-input" name="PermissionType" id="NormalPermission" value="Normal">
                                    Ücretsiz İzin
                                    <i class="input-helper"></i>
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-check">
                                <label class="form-check-label">
                                    <input type="radio" class="form-check-input" name="PermissionType" id="YearlyPermission" value="Yearly" checked>
                                    Yıllık İzin
                                    <i class="input-helper"></i>
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-check">
                                <label class="form-check-label">
                                    <input type="radio" class="form-check-input" name="PermissionType" id="ExcusedPermission" value="Excused">
                                    Mazaret İzni
                                    <i class="input-helper"></i>
                                </label>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-check">
                                <label class="form-check-label">
                                    <input type="radio" class="form-check-input" name="PermissionType" id="EqualizationPermission" value="Equalization">
                                    Denkleştirme İzni
                                    <i class="input-helper"></i>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`,
            icon: 'question',
            iconColor: '#008000',
            showCancelButton: true,
            confirmButtonText: 'Evet, kabul et!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary',

            },
            didOpen: () => {
                document.querySelector('.swal2-html-container').style.maxWidth = '10000px';
            },
            padding: 10,
            buttonsStyling: false,
        }).then(function (result) {
            if (result.value) {
                var permissionType = $('input[name="PermissionType"]:checked').val();
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value, PermissionType: permissionType },
                    success: function (response) {
                        eval(response);
                    }
                });
            }
        });
        return false;
    }

    if (method == "RejectPermission") {
        Swal.fire({
            title: 'Reddetme Onayı!',
            input: "textarea",
            inputAttributes: {
                autocapitalize: "off"
            },
            text: "Seçili izin reddedilecek. Aşağıdaki alana reddetme sebebinizi belirtebilirsiniz.",
            icon: 'question',
            iconColor: '#ea5455',
            showCancelButton: true,
            confirmButtonText: 'Evet, reddet!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            //var reason = $('.swal-content__textarea').val();
            var reason =  result.value
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value, rejectReason: reason },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "Print") {
        Swal.fire({
            title: 'Kabul Onayı!',
            text: "Seçili verinin belgesi çıkarılsın mı?",
            icon: 'question',
            iconColor: '#008000',
            showCancelButton: true,
            confirmButtonText: 'Evet, kabul et!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "GetBackToWork") {
        Swal.fire({
            title: 'Kabul Onayı!',
            text: "Seçili kişi işe geri alınsın mı?",
            icon: 'question',
            iconColor: '#008000',
            showCancelButton: true,
            confirmButtonText: 'Evet, geri al!',
            cancelButtonText: 'Vazgeç',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            padding: 10,
            buttonsStyling: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    type: "POST",
                    url: "/" + url,
                    data: { Id: value },
                    success: function (response) {
                        eval(response);
                    }

                });
            }
        });
        return false;
    }

    if (method == "DetailedPage") {
        $.ajax({
            type: "POST",
            url: "/" + url,
            data: { guid: value },
            success: function (response) {
                $('body').html(response);
            }

        });
        return false;
    }


    $.ajax({
        type: "POST",
        url: "/" + url,
        data: { guid: value },
        success: function (response) {
            eval(response);
        }

    });
}

function CallTable(url) {

    $('.dataTable').DataTable().clear().draw();

    //$('.dataTable').DataTable({
    //    language: {
    //        url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json",
    //    },
    //});

    $.ajax({
        type: "POST",
        url: "/" + url,
    });
}

$('.modal').on('show.bs.modal', function () {

    if (!$(this).attr('id').includes('Update') && !$(this).attr('id').includes('Detail')) {
        // Formu resetlemek için
        $(this).find('form')[0].reset();
        // Select2 dropdownları sıfırlamak için
        $(this).find('select.select2').val('0').trigger('change');  
    } 
   
});

function downloadURI(uri) {
    var link = document.createElement("a");
    link.href = uri;
    link.click();
}


