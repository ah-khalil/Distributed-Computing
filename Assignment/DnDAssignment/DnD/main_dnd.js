var subsect_arr;

window.onload = init;

function init(){
    init_subsect_arr();
    init_default_val();
    init_requests();
    init_buttons();
    init_hide();
}

function init_subsect_arr(){
    subsect_arr = [
        document.getElementById("character-vc-container"),
        document.getElementById("character-list-container")
    ];
}

function init_default_val(){
    
}

function init_requests(){
    var get_races_xmlreq;
    var get_class_xmlreq;
    
    get_races_xmlreq = new XMLHttpRequest();
    get_races_xmlreq.open('GET', 'DnDAssignment/Races/', true);
    get_races_xmlreq.onreadystatechange = function() {
        add_lists(document.getElementById('race-select'), get_races_xmlreq);
    };

    get_class_xmlreq = new XMLHttpRequest();
    get_class_xmlreq.open('GET', 'DnDAssignment/Classes/', true);
    get_class_xmlreq.onreadystatechange = function(){
        add_lists(document.getElementById('class-select'), get_class_xmlreq);
    };
    
    get_races_xmlreq.send();
    get_class_xmlreq.send();
}

function init_buttons(){
    var view_char_btn = null, view_chars_btn = null, create_char_btn = null;
    
    view_char_btn = document.getElementById("view-menu-btn");
    view_chars_btn = document.getElementById("view-all-menu-btn");
    create_char_btn = document.getElementById("create-menu-btn");
    
    view_char_btn.setAttribute('onclick', 'unhide_subsect(0)');
    view_chars_btn.setAttribute('onclick', 'unhide_subsect(1)');
    create_char_btn.setAttribute('onclick', 'unhide_subsect(0)');
}

function init_hide(){
    var subsection;
    
    if(subsect_arr.length < 1)
        return;
    
    for(var i = 0; i < subsect_arr.length; i++){
        subsection = subsect_arr[i];
        subsection.setAttribute('style', 'display:none;');
    }
}

function unhide_subsect(subsect_arr_id){
    var subsection;
    
    if(subsect_arr_id == null)
        return;
    
    if(subsect_arr_id == 0 && subsect_arr[1].style.display == '')
        subsect_arr[1].setAttribute('style', 'display:none');
    else if(subsect_arr_id == 1 && subsect_arr[0].style.display == '')
        subsect_arr[0].setAttribute('style', 'display:none');

    subsect_arr[subsect_arr_id].setAttribute('style', 'display:');
}

function add_lists(select_elem, xhreq){
    var option;
    var json_msg = {};
    var list_arr = [];
    
    console.log(xhreq.readyState);
    console.log(xhreq.status);
    console.log(select_elem);
    
    if(xhreq.readyState == '4' && xhreq.status == '200'){
        json_msg = JSON.parse(JSON.parse(xhreq.responseText));
        list_arr = json_msg["results"];
        
        for(var i = 0; i < list_arr.length; i++){
            option = document.createElement("option");
            option.value = list_arr[i]["name"];
            option.innerHTML = list_arr[i]["name"];
            select_elem.add(option, select_elem[i]);
        }
    }
}

function class_change(){
    var class_val;
    var level_val;
    var class_json;
    var class_hit_dice;
    var get_class_xmlreq;
    var select_elem, hit_dice_elem;

    select_elem = document.getElementById("class-select");
    hit_dice_elem = document.getElementById("class-hit-dice-val-hdn");
    class_val = select_elem.value;
    
    if(class_val != "" && class_val != null){
        get_class_xmlreq = new XMLHttpRequest();
        get_class_xmlreq.open('GET', 'Classes/' + parseInt(select_elem.selectedIndex + 1), false);
        get_class_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4' && this.status == '200'){
                class_json = JSON.parse(this.responseText);
                class_hit_dice = class_json["hit_die"];
                
                if("spellcaster" in class_json)
                    document.getElementById('spellcaster-radio-true').checked = True;
                else
                    document.getElementById('spellcaster-radio-false').checked = True;
                
                hit_dice_elem.value = class_hit_dice;
            }
        };
    }
}

function race_change(){
    var race_val;
    var race_json;
    var select_elem;
    var get_race_xmlreq;
    var racial_bonus_arr = [];
    var racial_bonus_p_elem_arr = [];

    select_elem = document.getElementById("race-select");
    race_val = select_elem.value;

    if(race_val != "" && race_val != null){
        get_race_xmlreq = new XMLHttpRequest();
        get_race_xmlreq.open('GET', 'Races/' + parseInt(select_elem.selectedIndex + 1), false);
        get_race_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4' && this.status == '200'){
                race_json = JSON.parse(this.responseText);
                racial_bonus_arr = race_json["ability_bonuses"];
                racial_bonus_p_elem_arr = document.getElementsByClassName("racial-bonus-p");

                for(var i = 0; i < racial_bonus_arr.length; i++){
                    if(racial_bonus_arr[i] > 0)
                        racial_bonus_p_elem_arr[i].innerHTML = "+" + racial_bonus_arr[i] + " racial bonus";
                    else
                        racial_bonus_p_elem_arr[i].innerHTML = "";
                }
            }
        }
    }
}

function update_hp(){
    var level_val, class_hit_dice_val, constt_val;
    
    level = parseInt(document.getElementById("level-num").value);
    constt_val = parseInt(document.getElementById("constt-rbp-val").value);
    class_hit_dice_val = parseInt(document.getElementById("class-hit-dice-val-hdn").value);
    
    document.getElementById("hit-points-num").value = level * class_hit_dice_val + constt_val;
}