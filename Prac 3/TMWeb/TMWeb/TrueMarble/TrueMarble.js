 var max_x, max_y;
 
 window.onload = init;

function init(){
    var req_xhr_zoom;
        
    req_xhr_zoom = new XMLHttpRequest();
    req_xhr_zoom.onreadystatechange = init_zoom_levels;
    req_xhr_zoom.open("GET", "/TMWeb/Zoom", true);
    req_xhr_zoom.send();
}

function init_zoom_levels(){
    var option;
    var select_elem;
    var req_xhr_img;
    var ret_val, med_val;
    var curr_x_val, curr_y_val;
      
    if(this.readyState == 4 && this.status == 200){
        med_val = 0;
        ret_val = JSON.parse(this.responseText);
        select_elem = document.getElementById("zoom_select");
        
        for(var i = 0; i < ret_val; i++){
            option = document.createElement("option");
            option.value = i + 1;
            option.innerHTML = i + 1;
            select_elem.add(option, select_elem[i]);                    
        }
        
        med_val = parseInt((ret_val + 1)/ 2);
        select_elem.value = med_val;
        curr_x_val = document.getElementById("x_container_hidden").value;
        curr_y_val = document.getElementById("y_container_hidden").value;
        
        req_xhr_img = new XMLHttpRequest();
        req_xhr_img.onreadystatechange = init_image;
        req_xhr_img.open("GET", "/TMWeb/" + med_val + "/" + curr_x_val + "/" + curr_y_val, true);
        req_xhr_img.send();
    }
}

function init_image(){
    var ret_val;
    var img_elem;
    var curr_zoom;
    var req_xhr_max_xy;
    
    if(this.readyState == 4 && this.status == 200){
        console.log(this.responseText);
        
        curr_zoom = document.getElementById("zoom_select").value;

        ret_val = JSON.parse(this.responseText);
        img_elem = document.getElementById("map_img");
        img_elem.setAttribute("src", "data:image/jpg;base64, " + ret_val); 
        
        req_xhr_max_xy = new XMLHttpRequest();
        req_xhr_max_xy.onreadystatechange = init_max_xy;
        req_xhr_max_xy.open("GET", "/TMWeb/zoom/" + curr_zoom, true);
        req_xhr_max_xy.send();
    }
}

function init_max_xy(){
    var ret_val;

    if(this.readyState == 4 && this.status == 200){
        ret_val = JSON.parse(this.responseText);
        max_x = ret_val["across"];
        max_y = ret_val["down"];
        
        console.log("Max X: " + max_x);
        console.log("Max Y: " + max_y)
    } else {
        max_x = 0;
        max_y = 0;
    }
}
    
function update_image(zoom_level, x_val, y_val){
    var req_xhr_img;
    
    req_xhr_img = new XMLHttpRequest();
    req_xhr_img.onreadystatechange = function(){
        if(this.readyState == 4 && this.status == 200){
            console.log(this.responseText);
        
            ret_val = JSON.parse(this.responseText);
            img_elem = document.getElementById("map_img");
            img_elem.setAttribute("src", "data:image/jpg;base64, " + ret_val);
        }
    }

    req_xhr_img.open("GET", "/TMWeb/" + zoom_level + "/" + x_val + "/" + y_val, true);
    req_xhr_img.send();
}

function set_max_xy(zoom_level){
    var req_xhr_max_xy;
    
    req_xhr_max_xy = new XMLHttpRequest();
    req_xhr_max_xy.onreadystatechange = function(){
        var ret_val;
        
        if(this.readyState == 4 && this.status == 200){
            ret_val = JSON.parse(this.responseText);
            max_x = ret_val["across"];
            max_y = ret_val["down"];
            
            console.log("Max X: " + max_x);
            console.log("Max Y: " + max_y)
        }
    };
    
    req_xhr_max_xy.open("GET", "/TMWeb/zoom/" + zoom_level, true);
    req_xhr_max_xy.send();
}
    
function update_zoom_level(){
    var curr_zoom, curr_x_val, curr_y_val;
    
    curr_zoom = document.getElementById("zoom_select").value;
    curr_x_val = document.getElementById("x_container_hidden").value;
    curr_y_val = document.getElementById("y_container_hidden").value;

    if (curr_x_val > max_x - 1)
        curr_x_val = parseInt((max_x - 1) / 2);
    else if (curr_x_val < 0)
        curr_x_val = 0;

    if (curr_y_val > max_y - 1)
        curr_y_val = parseInt((max_y - 1) / 2);
    else if (curr_y_val < 0)
        curr_y_val = 0;
    
    update_image(curr_zoom, curr_x_val, curr_y_val);
    set_max_xy(curr_zoom);
}

function change_pos(dir){
    var pos_changed = false;
    var curr_x_val, curr_y_val, curr_zoom;
    
    curr_zoom = document.getElementById("zoom_select").value;
    curr_x_val = document.getElementById("x_container_hidden").value;
    curr_y_val = document.getElementById("y_container_hidden").value;

    switch(dir){
        case "x+":
            if(curr_x_val < max_x - 1){
                curr_x_val++;
                document.getElementById("x_container_hidden").value = curr_x_val;
                pos_changed = true;
            }
                
            break;
        case "x-":
            if(curr_x_val > 0){
                curr_x_val--;
                document.getElementById("x_container_hidden").value = curr_x_val;
                pos_changed = true;
            }
                
            break;
        case "y+":
            if(curr_y_val < max_y - 1){
                curr_y_val++;
                document.getElementById("y_container_hidden").value = curr_y_val;
                pos_changed = true;
            }
                
            break;
        case "y-":
            if(curr_y_val > 0){
                curr_y_val--;
                document.getElementById("y_container_hidden").value = curr_y_val;
                pos_changed = true;
            }
                
            break;
    }
    
    if(pos_changed)
        update_image(curr_zoom, curr_x_val, curr_y_val);
}