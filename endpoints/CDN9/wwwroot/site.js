var myModal = new bootstrap.Modal(document.getElementById('myModal'));
var detailModal = new bootstrap.Modal(document.getElementById('detailModal'));
var uploadModal = new bootstrap.Modal(document.getElementById('uploadModal'));
var lastFolder = "";
var lastI0 = 0;
var lastFiles = [];
var baseurl = "home";
var icon_image = '🖼️';
var icon_video = '🎞️';
var icon_txt = '📄';
var icon_doc = '📘';
var icon_zip = '🗜';
var icon_audio = '🔊';
var icon_unknown = '⛘';
var icon_pdf = '📕';
var icon_excel = '📗';
var icon_powerpoint = '📙';
var icon_exe = '⚙️'
var icon_remove = '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAABdElEQVR4nN2VTU4CQRCFv0BgI7qDyBXAO6gXMGgMVzAQFfQQhngMFT2PYERRD2FwIRvGdPI6qYw9P/iz4SWdTOpVdXW/rqqBVUcZaAO3wBPwoeW+h+Kcz49wALwBUcZ6BfaX2bgAXJoN7oFToAGsaTWBHjAyfgPFZsJv/gkcZQQ5riNfnyRTFr/59hK33jFJWklOZaO5O7lHB6gF/GviPLqKfQFKoQRto3khFjSOJanJFsnHoWhsh6EEdyJPjK1qHnIC1AO2TePfl/0mlOBZZCMgxdjcxH7HpWuKc33yDTORlQBXjZVk/OQe6+Jnf5GgHvDbEP/+XxJtpUk0FNlLOPlEsoQe3uNc9qu0Mh39okwf0sq0rMEVxRqok7PRjhU7TWo0NBX9qHDtnxe7wBxYAHtZzgOTpKurJ6Gok88Vc5HnNAWTJJLWfTVRRctVy5nRfKHNc41rj5YGV9YPZ5pHliSUVBFutriSdM3o1iNwLS7xQVcDX3EsmNmCOciWAAAAAElFTkSuQmCC" alt="cancel--v1" height="20" >';
var icon_rename = '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAuElEQVR4nO3VMWrCQRDF4c8jeAU9jZ5BUHMDtbBUO1tFPU6wELETKwVzgmCnQpokImwlupCwfxD1wese82N2dmd5KQNV8YkxSlhinRJQxBBf+MUCbzLQKADyMlIXP1kV/xeggUNo+5Y/IoBtyJxrtK4B9pijH3E9AqiHzAzHa4Bv9BIcUSd0knsBun8FnIc8RTviSgRQCZn3W0NOeU2bHvIl382yK2Bwsa5rqT+cHSYoY4VNSoDn0gnwnVmtvLwDqQAAAABJRU5ErkJggg==" alt="rename--v1" height="20">';
function showFile2(path, title, extension) {

    extension = fileIcon(extension.toLowerCase());

    let fullPath = path + '/' + title;

    $("#showDetailModal").html(`<div class="mb-2 pb-1 px-2 text-left border-bottom">
        <div class="btn-group btn-group-sm" role="group" >
          <button type="button" onclick="prevFile('${fullPath}')" title="prev" class="btn btn-outline-primary"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAyklEQVR4nNXVIU4DQBAF0JcGUVksAoegAgmGI1TVkyDqwFWXO4DjDGDoDeAKvQACRWhS0dRAaNNkNmlQiJkE/gHeT3Z3dvjj6eGyCu/iGWuMs/EOHgN/w2F2wV3gHzjOxm8CX+E8Gx8F/oVhNj7AZxRcZeNnWAY+ycb7mAd+n40f4DXwJ+xl4vuYBf4Sg+VfFfw8omn2EbUc4b3qkltOd57pdpJLMtgZtOuqkgt8V30VLZPKz67lNkoWOFGQDh4qF47qldlSuvR/lQ2TBTV9NsnUKAAAAABJRU5ErkJggg==" alt="back" height="16"></button>
          <button type="button" onclick="nextFile('${fullPath}')" title="next" class="btn btn-outline-primary"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAwUlEQVR4nNXVoW2CYRAG4CdQgSwWUYegorKYjoBiAhJAIbEt3QEFM4ApG8AKLEACikCCaGogbc0JBvgvhBvgfZIv993LjaeDx6zwIf6wRCUDqGETyAIPGUgd+0CmkuYV34F8ZCEtnAMZZCG9AC5oZyGjQH7wloWMAznhJQMoYRbIDk8ZSAWrQNao3hVQxjzCtxlPNI7wAxpFh39mrmk/86O14lT8olt0ePPq2L0XHf6MY4RPMgvnK9bzvipTdukXMv9b7TV7hOCjPwAAAABJRU5ErkJggg==" alt="forward" height="16"></button>
        </div>

         <div class="btn-group btn-group-sm ms-5" role="group" >
          <button type="button" onclick="showRenameFileModal('${fullPath}','${title}')" title="rename" class="btn btn-outline-primary">${icon_rename}</button>
          <button type="button" onclick="removeFile('${fullPath}')" title="remove" class="btn btn-outline-danger">${icon_remove}</button>
        </div>



        <button onclick="pushSelectedfilesToParent('${fullPath}')" title="push up" class="btn btn-outline-primary btn-sm float-right" style="float:right" ><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAlElEQVR4nN3SMQ4BURCA4W9bjZLoRbelO0jECWzNATQSl+EEWkdAKxHiAg6gRTaZQqHY3afyJ5OZN8n8eS9v+DFdbCM3Gj7jGbmWpIMTVnhhgQt6VQU7LKN+oBXnsl+J9kd9Q/9LvzIbzCQwwR5ZU0GGI6Ypt8hxxzBFMgpJkfKcHIeIOQbxxbXIMMYa19iTctn+hTfMuxhJ1kicJwAAAABJRU5ErkJggg==" alt="up3" height="16"></button>
    </div>`);

    if (extension == icon_audio) {
        $("#showDetailModal").append(`<label class="mb-4">${fullPath}</label><br />
                                <a href="${fullPath}" target="_blank" class="mb-4">download⬇️</a> | <button onclick="playAudio('${fullPath}')" >play ${icon_audio}</button>`);

    }
    else if (extension == icon_image) {
        $("#showDetailModal").append(`<img src="${fullPath}" style="max-width: 100%;" />`);
    }
    else if (extension == icon_video) {
        $("#showDetailModal").append(`<label class="mb-4">${fullPath}</label><br />
                                <a href="${fullPath}" target="_blank" class="mb-4">download⬇️</a> | <button onclick="playVideo('${fullPath}')" >play ${icon_video}</button>`);
    }
    else {
        $("#showDetailModal").append(`<label class="mb-4">${fullPath}</label><br />
                                <a href="${fullPath}" target="_blank" class="mb-4">download⬇️</a>`);
    }

    $(".modal-title").html(title);
    detailModal.show();
}
function playAudio(fullPath) {
    $("#showDetailModal").append(`<audio controls class="w-100">
                                <source src="${fullPath}" type="audio/ogg">
                                <source src="${fullPath}" type="audio/mpeg">
                                Your browser does not support the audio element.
                            </audio>`);
}
function playVideo(fullPath) {
    $("#showDetailModal").append(`<video width="320" height="240" controls>
                              <source src="${fullPath}" type="video/mp4">
                              <source src="${fullPath}" type="video/ogg">
                              Your browser does not support the video tag.
                            </video>`);
}

function addDirectoryModal(path, title, dir) {

    $("#showModal").html(`
               <input type="hidden" class="form-control" id="parent_dir" value="${dir}" >
               <input type="hidden" class="form-control" id="parent_path" value="${path}" >
               <div class="mb-3 px-3">
                 <label for="exampleInputEmail1" class="form-label">Name</label>
                 <input type="text" class="form-control" id="dir_name" >
               </div>
               <button type="button" onclick="addSubDirectory()" class="btn btn-primary">Submit</button>
            `)
    $(".modal-title").html(title);
    myModal.show();
    $("#dir_name").focus();
}

function addSubDirectory() {
    $.post(`/${baseurl}/CreateDir`, { d: $("#parent_path").val(), name: $("#dir_name").val() })
        .done(function (data) {
            myModal.hide();
            lastFolder = "";
            let pd = $("#parent_dir").val();
            let pp = $("#parent_path").val();
            let newD = $("#dir_name").val();
            loadSub(pd.replace("dir_", ""), pp, () => {
                let a = "#" + pd + " [title=" + newD + "]"
                var elm = $(a)[0];


                $(elm).click();
                lastFolder = pp + "/" + newD;
                //loadSub(pd.replace("dir_", ""), pp + '/' + newD)
            });

            $("#showModal").html(``);
        })
        .fail(function () {
            alert("error");
        });
}

function removeDir(path, parentD, divId) {

    $.post(`/${baseurl}/RemoveDir`, { d: path })
        .done(function () {

            lastFolder = "";
            loadSub(divId, parentD)

        })
        .fail(function (er) {
            alert(er["System.IO.IOException"]);
        });
}

function removeFile(path, j) {

    $.post(`/${baseurl}/RemoveFile`, { d: path })
        .done(function () {
            $('#file_' + j).remove();
            //loadSub(_lastuploadFilePath.path, _lastuploadFilePath.d)
        })
        .fail(function (er) {
            alert(er["System.IO.IOException"]);
        });
}

function renameFile(path, j) {

    let newName = $("#file_name").val();

    $.post(`/${baseurl}/RenameFile`, { d: path, newName })
        .done(function () {
            myModal.hide();

            let _lastFolder = lastFolder;
            lastFolder = "";
            loadSub(lastI0, _lastFolder);
        })
        .fail(function (er) {
            alert(er["System.IO.IOException"]);
        });
}

function showRenameFileModal(path, name, j) {

    $("#showModal").html(`
               <div class="mb-3 px-3">
                 <label for="exampleInputEmail1" class="form-label">Name</label>
                 <input type="text" class="form-control" id="file_name" value='${name}' >
               </div>
               <button type="button" onclick="renameFile('${path}',${j})" class="btn btn-primary">Submit</button>
            `)
    $(".modal-title").html('');
    myModal.show();


    var input = $('#file_name')[0]; // Get raw DOM element
    var end = input.value.lastIndexOf('.');
    input.setSelectionRange(0, end);
    input.focus(); // Focus the input to make the selection visible
}

function loadSub(i0, d,onAfterLoad) {

    $(`.folder`).removeClass('open');
    if (lastFolder == d && d != '') {
        $(`#dir_${i0}`).html('');
        $("#files_").html('');
        $(`.d_${i0}`).removeClass('open');
        lastFolder = "";
        return;
    }
    $(`.d_${i0}`).addClass('open');
    lastFolder = d;
    lastI0 = i0;
    $("#fullPathOfDir").val(d);
    $("#files_").html('<div class="spinner-border" role="status"></div>');
    let isSearch = $('#search_files').val() != '';
    $.get(`/${baseurl}/SubDir?d=` + d + '&search=' + $('#search_files').val(), function (data, status) {

        lastFiles = data.files;
        let html = '';
        for (let i = 0; i < data.dirs.length; i++) {
            let j = i;
            html += `<div class="">
                            <div class=""  style="display:inline-flex;white-space: nowrap;">
                                <div class="dropdown">
                                    <button class="btn btn-link btn-sm" type="button"  data-bs-toggle="dropdown" >
                                        ⋮
                                    </button>
                                    <ul class="dropdown-menu" >
                                        <li><a class="dropdown-item" onclick="showUploadModal('${data.dirs[j].path}','${data.dirs[j].text}','${i0}_${j}')" href="#">📤Upload File</a></li>
                                        <li><a class="dropdown-item" onclick="addDirectoryModal('${data.dirs[j].path}','${data.dirs[j].text}','${i0}_${j}')" href="#">➕Sub Directory</a></li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li><a class="dropdown-item" onclick="removeDir('${data.dirs[j].path}','${d}','${i0}')" href="#">🗑️Remove</a></li>
                                    </ul>
                                </div>
                                <label class="p-2 ps-0 folder d_${i0}_${j}" onclick="loadSub('${i0}_${j}','${data.dirs[j].path}')" title="${data.dirs[j].text}">${data.dirs[j].text}</label>
                            </div>
                            <div id="dir_${i0}_${j}" class="ml-3 ms-3" style="" > </div>
                        </div>`;
        }
        if (d == '' || d == '/')
            $(".mainDirecories").html(html);
        else
            $("#dir_" + i0).html(html);


        html = '';
        for (let i = 0; i < data.files.length; i++) {
            let j = i;
            let f = data.files[j];
            html += `<tr id="file_${j}">
                               <td class="py-1 px-2" style="display:inline-flex">
                                   <label style='white-space: nowrap;'>
                                      <input type="checkbox" value='${f.path}/${f.text}' class="_files" />
                                      ${fileIcon(f.extension)}
                                   </label>
                                   <label onclick="showFile2('${f.path}','${f.text}','${f.extension}')" id="file_lbl_${j}" title="(${f.length}) - ${f.path} " >
                                        ${f.text}
                                   </label>
                                   
                                   <div class="dropdown">
                                        <button class="btn btn-link btn-sm" type="button"  data-bs-toggle="dropdown" >
                                            ⋮
                                        </button>
                                        <ul class="dropdown-menu" >
                                            <li><a class="dropdown-item" onclick="showRenameFileModal('${f.path}/${f.text}','${f.text}',${j})" href="#">${icon_rename}Rename</a></li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li><a class="dropdown-item" onclick="removeFile('${f.path}/${f.text}',${j})" href="#">${icon_remove}Remove</a></li>
                                        </ul>
                                    </div>
                              </td>
                              <td><small>${f.length}</small></td>
                     </tr>`;
        }
        if (data.files.length == 0)
            $("#files_").html(`<div class="ml-4 pb-2">--folder is empty--</div>`);
        else
            $("#files_").html(html);

        let Length = data.files.reduce((n, { lengthByte }) => n + lengthByte, 0);
        $(".total-file-count").html(data.files.length);
        $(".total-file-size").html(Math.ceil(Length < 1024 ? Length : Length < 1_000_000 ? Length / 1024 : Length / 1000000));
        $(".total-file-style").html(Length < 1024 ? 'B' : Length < 1_000_000 ? 'Kb' : 'Mb');

        if (onAfterLoad != undefined)
            onAfterLoad();
    });
}

function fileIcon(extension) {
    if (extension.match(/\.(jpg|jpeg|png|gif|webp|bmp|svg)$/i)) return icon_image;
    if (extension.match(/\.(mp4|m4a|avi|mov|f4v)$/i)) return icon_video;
    if (extension.match(/\.(docx|doc)$/i)) return icon_doc;
    if (extension.match(/\.(xlsx|xls)$/i)) return icon_excel;
    if (extension.match(/\.(pptx|ppt)$/i)) return icon_powerpoint;
    if (extension.match(/\.(txt)$/i)) return icon_txt;
    if (extension.match(/\.(rar|zip|7zip)$/i)) return icon_zip;
    if (extension.match(/\.(mp3)$/i)) return icon_audio;
    if (extension.match(/\.(pdf)$/i)) return icon_pdf;
    if (extension.match(/\.(exe)$/i)) return icon_exe;
    return icon_unknown;
}

function pushSelectedfilesToParent(filePath) {
    window.parent.postMessage(filePath, '*');
    myModal.hide();
}

function nextFile(filePath) {    
    let ind = lastFiles.findIndex(x => x.path+'/'+x.text == filePath);
    ind++;
    if (ind == lastFiles.length)
        return;
    let f = lastFiles[ind];
    showFile2(`${f.path}`, `${f.text}`, `${f.extension}`);
}

function prevFile(filePath) {
    let ind = lastFiles.findIndex(x => x.path + '/' + x.text == filePath);
    if (ind == 0)
        return;
    ind--;
    let f = lastFiles[ind];
    showFile2(`${f.path}`, `${f.text}`, `${f.extension}`);
}

function getSelectedFilesandPushToParent() {
    var checkedValue = '';
    var inputElements = document.getElementsByClassName('_files');

    for (var i = 0; inputElements[i]; ++i)
        if (inputElements[i].checked)
            checkedValue += inputElements[i].value + '**#**';

    pushSelectedfilesToParent(checkedValue);
}

function togglecheckAllByClass() {
    const checkboxes = document.querySelectorAll(`._files`);
    checkboxes.forEach(checkbox => {
        checkbox.checked = document.getElementById('check_all_file').checked;
    });
}

var _lastuploadFilePath;
//function uploadFile(path, d, i0) {
//    _lastuploadFilePath = { path, d, i0 };
//    document.getElementById('lastuploadFileInput').click();
//}
function showUploadModal(path, d, i0) {
    _lastuploadFilePath = { path, d, i0 };
    $(".modal-title").html(path);
    uploadModal.show();
}
