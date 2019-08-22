const column_name_to_fld_id_map = {
    "name"          : "character-name-inp",
    "age"           : "character-age-inp",
    "gender"        : "character-gender-inp",
    "biography"     : "character-biography-inp",
    "level"         : "character-level-inp",
    "race"          : "race-select",
    "class"         : "class-select",
    "spellcaster"   : "spellcaster-radio-true",
    "hit_points"    : "hit-points-num",
    "ab_str"        : "str-rbp-val",
    "ab_con"        : "con-rbp-val",
    "ab_dex"        : "dex-rbp-val",
    "ab_cha"        : "cha-rbp-val",
    "ab_int"        : "int-rbp-val",
    "ab_wis"        : "wis-rbp-val"
};

window.onload = init;

function init(){
    init_requests();
    init_buttons();
    init_hide();
}

function prepare_view(){
    var curr_json_string;
    var get_races_xmlreq = new XMLHttpRequest();
    var get_class_xmlreq = new XMLHttpRequest();
    
    curr_json_string = document.getElementById('current-character-json').value;
    document.getElementById('delete-btn-container').setAttribute('style', 'display:;');
    document.getElementById('create-xml-btn-container').setAttribute('style', 'display:;');

    get_races(get_races_xmlreq, function(){
        if(this.readyState == '4'){
            if(this.status == '200'){
                toggle_disable_fields(true);
                set_char_fields(curr_json_string);

                update_tss();
                race_change();
                class_change();

                set_action_button_type('view');
                unhide_subsect(0);
            } else if(this.status == '500'){
                alert(this.responseText);
            }
        }
    });
}

function prepare_edit(){
    var curr_json_string;

    curr_json_string = document.getElementById('current-character-json').value;
    document.getElementById('delete-btn-container').setAttribute('style', 'display:none;');
    document.getElementById('create-xml-btn-container').setAttribute('style', 'display:none;');
    
    toggle_disable_fields(false);
    set_char_fields(curr_json_string);
    update_hp();
    update_tss();
    update_total();
    set_action_button_type("edit");
    unhide_subsect(0);
}

function prepare_create(){
    document.getElementById('delete-btn-container').setAttribute('style', 'display:none;');
    document.getElementById('create-xml-btn-container').setAttribute('style', 'display:none;');

    reset_fields();
    update_tss();
    update_total();
    toggle_disable_fields(false);
    set_action_button_type("create");
    unhide_subsect(0);
}

function prepare_table(){
    unhide_subsect(1);
}

function init_buttons(){
    var view_char_btn, view_chars_btn, create_char_btn, delete_char_btn, xml_down_btn;
    
    view_char_btn = document.getElementById("view-menu-btn");
    view_chars_btn = document.getElementById("view-all-menu-btn");
    create_char_btn = document.getElementById("create-menu-btn");
    delete_char_btn = document.getElementById('delete-btn');
    xml_down_btn = document.getElementById('create-xml-btn');
    
    view_chars_btn.setAttribute('onclick', 'prepare_table();');
    view_char_btn.setAttribute('onclick', 'get_character();');
    create_char_btn.setAttribute('onclick', 'prepare_create();');
    delete_char_btn.setAttribute('onclick', 'delete_char();');
    xml_down_btn.setAttribute('onclick', 'get_xml_download();');
}

function init_hide(){
    var subsect_arr;
    
    subsect_arr = [
        document.getElementById("character-vc-container"),
        document.getElementById("character-list-container")
    ];

    if(subsect_arr.length < 1)
        return;
    
    for(var i = 0; i < subsect_arr.length; i++){
        subsect_arr[i].setAttribute('style', 'display:none;');
    }
}

function init_requests(){
    var get_races_xmlreq = new XMLHttpRequest();
    var get_class_xmlreq = new XMLHttpRequest();
    
    get_races(get_races_xmlreq, function(){
        add_lists(document.getElementById('race-select'), get_races_xmlreq)
    });
    
    get_classes(get_class_xmlreq, function(){
        add_lists(document.getElementById('class-select'), get_class_xmlreq)
    });
    
    get_ability_types();
    get_characters();
}

function get_races(xmlreq, orsc_func){
    xmlreq.open('GET', 'DnDAssignment/Races/', true);
    xmlreq.onreadystatechange = orsc_func;
    xmlreq.send();
}

function get_classes(xmlreq, orsc_func){
    xmlreq.open('GET', 'DnDAssignment/Classes/', true);
    xmlreq.onreadystatechange = orsc_func;
    xmlreq.send();
}

function get_ability_types(){
    var get_types_xmlreq;

    get_types_xmlreq = new XMLHttpRequest();
    get_types_xmlreq.open('GET', 'DnDAssignment/AbilityTypes', true);
    get_types_xmlreq.onreadystatechange = add_ability_types;

    get_types_xmlreq.send();
}

function get_characters(){
    var get_chars_xmlreq;

    get_chars_xmlreq = new XMLHttpRequest();
    get_chars_xmlreq.open('GET', 'DnDAssignment/Characters', true);
    get_chars_xmlreq.onreadystatechange = all_character_req_handler;

    get_chars_xmlreq.send();
}

function get_xml_download(){
    var curr_char;
    var json_string;
    var dl_link_a_elem;

    json_string = document.getElementById('current-character-json').value;
    json_string = json_string.replace(/\\'/g, "'").replace(/\\"/g, '"');
    curr_char = JSON.parse(json_string);
    
    dl_link_a_elem = document.getElementById('download-link-a');
    dl_link_a_elem.href = 'DnDAssignment/Characters/XML/' + parseInt(curr_char["id"]);
    dl_link_a_elem.click();
}

function get_character(){
    var search_name;
    var get_char_xmlreq;
    
    search_name = document.getElementById('character-input-btn').value;
    
    if(search_name != "" && search_name != null){
        get_char_xmlreq = new XMLHttpRequest();
        get_char_xmlreq.open('POST', 'DnDAssignment/Characters/Name', true);
        get_char_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4'){
                if(this.status == '200'){
                    if(JSON.parse(this.responseText) == "{}")
                        alert("The input character was not found");
                    else{
                        set_hdn_curr_char(JSON.parse(this.responseText));
                        prepare_view();
                    }
                } else if(this.status == '500'){
                    alert(this.responseText);
                }
            }
        };
        
        get_char_xmlreq.setRequestHeader('Content-type', 'application/json');
        get_char_xmlreq.setRequestHeader('Response-type', 'application/json');
        get_char_xmlreq.send(JSON.stringify({"name" : search_name}));
    } else
        alert("Please enter a name");
}

function get_char_field_data(){
    return {
        "name"              : document.getElementById('character-name-inp').value,
        "age"               : document.getElementById('character-age-inp').value,
        "gender"            : document.getElementById('character-gender-inp').value,
        "biography"         : document.getElementById('character-biography-inp').value,
        "level"             : document.getElementById('character-level-inp').value,
        "race"              : document.getElementById('race-select').value,
        "class"             : document.getElementById('class-select').value,
        "spellcaster"       : ~~(document.getElementById('spellcaster-radio-true').checked),
        "hit_points"        : document.getElementById('hit-points-num').value,
        "ab_con"            : document.getElementById('con-rbp-val').value,
        "ab_dex"            : document.getElementById('dex-rbp-val').value,
        "ab_str"            : document.getElementById('str-rbp-val').value,
        "ab_cha"            : document.getElementById('cha-rbp-val').value,
        "ab_int"            : document.getElementById('int-rbp-val').value,
        "ab_wis"            : document.getElementById('wis-rbp-val').value
    };
}

function add_char_to_table(json_char){
    var view_a_elem;
    var esc_json_str;
    var child_tr_elem;
    var table_body_elem;
    var tbl_clmn_prop_arr;
    var child_txt_elem, view_txt_elem;
    var child_td_elem, view_td_elem;
    
    if(json_char == null)
        return false;
    
    try{
        child_tr_elem = document.createElement('tr');
        table_body_elem = document.getElementById('character-list-body-container');
        tbl_clmn_prop_arr = ["id", "name", "race", "class", "level"];

        for(var i = 0; i < tbl_clmn_prop_arr.length; i++){
            child_td_elem = document.createElement('td');
            child_txt_elem = document.createTextNode(json_char[tbl_clmn_prop_arr[i]]);

            child_td_elem.appendChild(child_txt_elem);
            child_tr_elem.appendChild(child_td_elem);
        }

        view_txt_elem = document.createTextNode('View');
        view_a_elem = document.createElement('a');
        view_td_elem = document.createElement('td');

        esc_json_str = JSON.stringify(json_char).replace(/\'/g, "\\'").replace(/\"/g, '\\"');

        view_a_elem.setAttribute('href', 'javascript:void(0)');
        view_a_elem.setAttribute('onclick', 'set_hdn_curr_char(\'' + esc_json_str + '\'); prepare_view();');

        view_a_elem.appendChild(view_txt_elem);
        view_td_elem.appendChild(view_a_elem);

        child_tr_elem.appendChild(view_td_elem);
        table_body_elem.appendChild(child_tr_elem);   
    } catch(err){
        alert("Something went wrong while creating list of characters");
    }
}

function add_lists(select_elem, xhreq){
    var option;
    var url_idx;
    var url_idx_split;
    var json_msg = {};
    var list_arr = [];

    if(xhreq.readyState == '4'){
        if(xhreq.status == '200'){
            json_msg = JSON.parse(JSON.parse(decode_str(xhreq.responseText)));
            list_arr = json_msg["results"];

            select_elem.innerText = null;

            for(var i = 0; i < list_arr.length; i++){
                url_idx_split = list_arr[i]["url"].split("/");
                url_idx = url_idx_split[url_idx_split.length - 1];

                option = document.createElement("option");
                option.value = list_arr[i]["name"];
                option.innerHTML = list_arr[i]["name"];
                select_elem.add(option, select_elem[url_idx - 1]);
            }
        } else if(xhreq.status == '500'){
            alert(xhreq.responseText);
        }
    }
}

function add_ability_types(){
    var json_msg;
    var html_str = "";
    var total_ab_scores;
    var ability_type_arr;
    var constt_score_num_elem;
    var ability_score_tbl_elem;
    var shorthand_to_name_map = {"STR" : "Strength", "DEX" : "Dexterity", "CON" : "Constitution", "INT" : "Intelligence", "WIS" : "Wisdom", "CHA" : "Charisma"};
    var shorthand_to_id_map = {"STR" : "str-rbp-val", "DEX" : "dex-rbp-val", "CON" : "con-rbp-val", "INT" : "int-rbp-val", "WIS" : "wis-rbp-val", "CHA" : "cha-rbp-val"};
    
    try{
        if(this.readyState == '4'){
            if(this.status == '200'){
                json_msg = JSON.parse(JSON.parse(decode_str(this.responseText)));
                total_ab_scores = json_msg["count"];
                ability_type_arr = json_msg["results"];
                ability_score_tbl_elem = document.getElementById('ability-score-val-tbl');
                
                for(var i = 0; i < total_ab_scores; i++){
                    if(shorthand_to_id_map[ability_type_arr[i]["name"]] != null && shorthand_to_id_map[ability_type_arr[i]["name"]] != ""){
                        html_str += "<tr>";
                        html_str += "   <td><p><i>" + shorthand_to_name_map[ability_type_arr[i]["name"]] + ":</i></p></td>";
                        html_str += "   <td><input type='number' id='" + shorthand_to_id_map[ability_type_arr[i]["name"]] +"' value='0' onchange='update_tss(); update_total();'></td>";
                        html_str += "   <td><p class='racial-bonus-p'></p></td>";
                        html_str += "</tr>";
                    }
                }

                html_str += "<tr><td><p id='character-rbp-inp-err-msg' class='error-msg-text'></p></td></tr>";
                html_str += "<tfoot><tr><td><p><b> Total Selected Score: </b></p></td>";
                html_str += "<td><p id='rbp-total-selected-p' class='rbp-total-p'>0</p></td></tr>";
                html_str += "<tr><td><p><b> Total Score: </b></p></td>";
                html_str += "<td><p id='rbp-total-true-p' class='rbp-total-p'>0</p></td></tr></tfoot>";

                ability_score_tbl_elem.innerHTML = html_str;
                constt_score_num_elem = document.getElementById('con-rbp-val');

                if(constt_score_num_elem != null)
                    constt_score_num_elem.setAttribute('onchange', 'update_tss(); update_total(); update_hp();');
            } else if(this.status == '500'){
                alert(this.responseText);
            }
        }
    } catch(err){
        alert("Something went wrong while retrieving ability types");
    }
}

function reset_fields(){
    var rbp_fld_arr;
    var input_fld_arr;
    var select_fld_arr;
    var hdn_hit_die_fld;
    var txtarea_fld_elem;
    
    try{
        input_fld_arr = document.getElementsByTagName('input');
        select_fld_arr = document.getElementsByTagName('select');
        rbp_fld_arr = document.getElementsByClassName('racial-bonus-p');
        hdn_hit_die_fld = document.getElementById('class-hit-dice-val-hdn');
        txtarea_fld_elem = document.getElementById('character-biography-inp');

        for(var i = 0; i < input_fld_arr.length; i++){
            if(input_fld_arr[i].type == "number")
                input_fld_arr[i].value = "0";
            else if(input_fld_arr[i].type == "radio")
                input_fld_arr[i].checked = false;
            else
                input_fld_arr[i].value = "";
        }

        for(var j = 0; j < select_fld_arr.length; j++){
            select_fld_arr[j].add(document.createElement("option"), select_fld_arr[j].length);
            select_fld_arr[j].selectedIndex = select_fld_arr[j].length;
        }

        for(var k = 0; k < rbp_fld_arr.length; k++){
            rbp_fld_arr[k].innerText = "";
        }

        hdn_hit_die_fld.value = "0";
        txtarea_fld_elem.value = "";
    } catch(err){
        alert("Something went wrong while resetting input fields");
    }
}

function toggle_disable_fields(to_disable){
    var curr_fld_elem;
    var char_bio_elem;
    var char_info_tbl_elem;
    var input_fld_elem_arr = [];
    var select_fld_elem_arr = [];
    var combined_fld_elem_arr = [];
    
    try{
        char_bio_elem = document.getElementById('character-biography-inp');
        char_info_tbl_elem = document.getElementById('character-vc-container-table');
        input_fld_elem_arr = Array.from(char_info_tbl_elem.getElementsByTagName('input'));
        select_fld_elem_arr = Array.from(char_info_tbl_elem.getElementsByTagName('select'));
        combined_fld_elem_arr = input_fld_elem_arr.concat(select_fld_elem_arr);

        for(var i = 0; i < combined_fld_elem_arr.length; i++){
            curr_fld_elem = combined_fld_elem_arr[i];

            if(curr_fld_elem != null && ["spellcaster-radio-false", "spellcaster-radio-true", "hit-points-num"].indexOf(curr_fld_elem.id) == -1)
                curr_fld_elem.disabled = to_disable;
        }

        char_bio_elem.disabled = to_disable;   
    } catch(err){
        alert("Something went wrong while toggling disability of input fields");
    }
}

function unhide_subsect(subsect_arr_id){
    var subsect_arr;
    
    try{
        subsect_arr = [
            document.getElementById("character-vc-container"),
            document.getElementById("character-list-container")
        ];

        if(subsect_arr_id == null)
            return;

        for(var i = 0; i < subsect_arr.length; i++){
            if(i != subsect_arr_id)
                subsect_arr[i].setAttribute('style', 'display:none;');
        }

        subsect_arr[subsect_arr_id].setAttribute('style', 'display:');   
    } catch(err){
        alert("Something went wrong while unhiding user interface");
    }
}

function set_hdn_curr_char(json_str){
    var curr_char_hdn_elem;
    
    curr_char_hdn_elem = document.getElementById('current-character-json');
    curr_char_hdn_elem.value = escape_json_keywords(json_str);
}

function set_char_fields(json_str){
    var json_char;
    var curr_elem;
    var curr_elem_name;
    var select_label_idx;
    var select_child_nodes_arr;
    
    try{
        json_str = json_str.replace(/\\'/g, "'").replace(/\\"/g, '"');
        json_char = JSON.parse(json_str);

        for(var prop in json_char){
            select_label_idx = -1;
            curr_elem_name = column_name_to_fld_id_map[prop];

            if(curr_elem_name != null && curr_elem_name != ""){
                curr_elem = document.getElementById(curr_elem_name);

                if(curr_elem != null) {
                    if(curr_elem.tagName == "SELECT"){
                        select_child_nodes_arr = curr_elem.getElementsByTagName('option');

                        for(var i = 0; i < select_child_nodes_arr.length && select_label_idx == -1; i++){
                            if(select_child_nodes_arr[i].value == json_char[prop])
                                select_label_idx = i;
                        }

                        curr_elem.selectedIndex = (select_label_idx > -1) ? select_label_idx : 0;
                    }
                    else if(curr_elem.tagName == "INPUT" && curr_elem.id == "spellcaster-radio-true")
                        curr_elem.checked = !!json_char[prop];
                    else if(curr_elem.tagName == "INPUT" && curr_elem.id == "spellcaster-radio-false")
                        curr_elem.checked = !json_char[prop];
                    else
                        curr_elem.value = json_char[prop];
                }
            }
        }   
    } catch(err){
        alert("An error occurred while setting character details");
    }
}

function set_action_button_type(type){
    var action_btn;
    
    try{
        if(type != null && type != ""){
            action_btn = document.getElementById('action-btn');

            if(type == "edit" || type == "create"){
                action_btn.innerText = 'Submit';
                action_btn.setAttribute('onclick', 'submit("' + type + '")');
            } else if(type == "view"){
                action_btn.innerText = 'Edit';
                action_btn.setAttribute('onclick', 'prepare_edit()');
            }
        }   
    } catch(err){
        alert("An error occurred while configuring buttons");
    }
}

function class_change(){
    var class_val;
    var class_json;
    var class_hit_dice;
    var get_class_xmlreq;
    var select_elem, hit_dice_elem;

    select_elem = document.getElementById("class-select");
    hit_dice_elem = document.getElementById("class-hit-dice-val-hdn");
    class_val = select_elem.value;
    
    if(class_val != "" && class_val != null){
        get_class_xmlreq = new XMLHttpRequest();
        get_class_xmlreq.open('GET', 'DnDAssignment/Classes/' + parseInt(select_elem.selectedIndex + 1), true);
        get_class_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4'){
                if(this.status == '200'){
                    try{
                        class_json = JSON.parse(JSON.parse(decode_str(this.responseText)));
                        class_hit_dice = class_json["hit_die"];

                        if("spellcasting" in class_json)
                            document.getElementById('spellcaster-radio-true').checked = true;
                        else
                            document.getElementById('spellcaster-radio-false').checked = true;

                        hit_dice_elem.value = class_hit_dice;
                        update_hp();
                    } catch(err){
                        alert("An error occurred while applying class settings");
                    }
                } else if(this.status == '500'){
                    alert(this.responseText);
                }
            }
        };
    }

    get_class_xmlreq.send();
}

function race_change(){
    var race_val;
    var race_json;
    var select_elem;
    var rbp_hdn_elem;
    var get_race_xmlreq;
    var racial_bonus_arr = [];
    var racial_bonus_p_elem_arr = [];

    select_elem = document.getElementById("race-select");
    race_val = select_elem.value;

    if(race_val != "" && race_val != null){
        get_race_xmlreq = new XMLHttpRequest();
        get_race_xmlreq.open('GET', 'DnDAssignment/Races/' + parseInt(select_elem.selectedIndex + 1), true);
        get_race_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4'){
                if(this.status == '200'){
                    try{
                        race_json = JSON.parse(JSON.parse(decode_str(this.responseText)));
                        racial_bonus_arr = race_json["ability_bonuses"];
                        rbp_hdn_elem = document.getElementById('rbp-hdn');
                        racial_bonus_p_elem_arr = document.getElementsByClassName("racial-bonus-p");

                        rbp_hdn_elem.value = 0;

                        for(var i = 0; i < racial_bonus_arr.length; i++){
                            if(racial_bonus_arr[i] > 0){
                                racial_bonus_p_elem_arr[i].innerHTML = "+" + racial_bonus_arr[i] + " racial bonus";
                            }
                            else
                                racial_bonus_p_elem_arr[i].innerHTML = "";

                            rbp_hdn_elem.value = parseInt(rbp_hdn_elem.value) +  parseInt(racial_bonus_arr[i]);
                        }

                        update_total();       
                    } catch(err){
                        alert("An error occurred while applying race settings");
                    }
                } else if(this.status == '500'){
                    alert(this.responseText);
                }
            }
        };
        
        get_race_xmlreq.send();
    }
}

function submit(type){
    var err_msg;
    var json_msg;
    var json_string;
    var curr_char_json;
    var post_charac_xmlreq;

    err_msg = validate_send();
    
    if(err_msg != ""){
        alert(err_msg);
        return false;
    }
    
    try{
        json_msg = get_char_field_data();
        post_charac_xmlreq = new XMLHttpRequest();

        json_msg["tts"] = document.getElementById('rbp-total-selected-p').innerText;

        if(type == "edit"){
            json_string = document.getElementById('current-character-json').value;
            json_string = json_string.replace(/\\'/g, "'").replace(/\\"/g, '"');
            curr_char_json = JSON.parse(json_string);
            json_msg["id"] = curr_char_json["id"];
            post_charac_xmlreq.open('PUT', 'DnDAssignment/Characters/' + parseInt(curr_char_json["id"]), true);
        } else if(type == "create")
            post_charac_xmlreq.open('POST', 'DnDAssignment/Characters', true);

        post_charac_xmlreq.onreadystatechange = function(){
            if(this.readyState == '4'){
                if(this.status == '200' || this.status == '204'){
                    get_characters();
                } else if(this.status == '400' || this.status == '500'){
                    alert(this.responseText);
                }
            }
        };

        post_charac_xmlreq.setRequestHeader('Content-type', 'application/json');
        post_charac_xmlreq.setRequestHeader('Response-type', 'application/json');
        post_charac_xmlreq.send(JSON.stringify(json_msg));
    } catch(err) {
        alert("An error occurred while attempting to submit character");
    }
}

function delete_char(){
    var json_string;
    var curr_char_json;
    var delete_char_xmlreq;
    
    try{
        if(confirm("Are you sure you would like to delete this character?")){
            json_string = document.getElementById('current-character-json').value;
            json_string = json_string.replace(/\\'/g, "'").replace(/\\"/g, '"');
            curr_char_json = JSON.parse(json_string);

            delete_char_xmlreq = new XMLHttpRequest();
            delete_char_xmlreq.open('DELETE', 'DnDAssignment/Characters/' + parseInt(curr_char_json["id"]), true);
            delete_char_xmlreq.onreadystatechange = function(){
                if(this.readyState == '4'){
                    if(this.status == '200' || this.status == '204'){
                        get_characters();
                    } else if(this.status == '500'){
                        alert(this.responseText);
                    }
                }
            };
            delete_char_xmlreq.send();
        }   
    } catch(err){
        alert("An error occurred while deleting character");
    }
}

function validate_send(){
    var fld_obj;
    var err_msg = "";
    
    try{
        fld_obj = get_char_field_data();

        if (fld_obj["name"] == "")
            err_msg += "Name must not be empty\n";

        if (fld_obj["age"] == "")
            err_msg += "Age must not be empty\n";
        else if (isNaN(fld_obj["age"]))
            err_msg += "Age must be a number\n";
        else if (fld_obj["age"] < 0 || fld_obj["age"] > 500)
            err_msg += "Age must be between 0 and 500\n";

        if (fld_obj["gender"] == "")
            err_msg += "Gender must not be empty\n";

        if (fld_obj["biography"] == "")
            err_msg += "Biography must not be empty\n";
        else if (fld_obj["biography"].length > 500)
            err_msg += "Biography must be less than 500 characters\n";

        if (fld_obj["ab_con"] == "" || fld_obj["ab_dex"] == "" || fld_obj["ab_str"] == "" || fld_obj["ab_cha"] == "" || fld_obj["ab_int"] == "" || fld_obj["ab_wis"] == "")
            err_msg += "Ability scores must not be blank (place a 0 for unapplied stats)\n";
        else if(isNaN(fld_obj["ab_con"]) || isNaN(fld_obj["ab_dex"]) || isNaN(fld_obj["ab_str"]) || isNaN(fld_obj["ab_cha"]) || isNaN(fld_obj["ab_int"]) || isNaN(fld_obj["ab_wis"]))
            err_msg += "Ability scores must be numbers\n";
        else if(fld_obj["ab_con"] < 0 || fld_obj["ab_dex"] < 0 || fld_obj["ab_str"] < 0 || fld_obj["ab_cha"] < 0 || fld_obj["ab_int"] < 0 || fld_obj["ab_wis"] < 0)
            err_msg += "Ability scores must be positive numbers\n";

        return err_msg;
    } catch(err){
        alert("An error occurred while validating input character information");
    }
}

function update_hp(){
    var level_val, class_hit_dice_val, constt_val;
    
    try{
        level_val = parseInt(document.getElementById("character-level-inp").value);
        constt_val = parseInt(document.getElementById("con-rbp-val").value);
        class_hit_dice_val = parseInt(document.getElementById("class-hit-dice-val-hdn").value);

        document.getElementById("hit-points-num").value = level_val * class_hit_dice_val + constt_val;   
    } catch(err){
        alert("An error occurred while updating hit points");
    }
}

function update_tss(){
    var tss_elem;
    var score = 0;
    var ab_score_elem_arr;

    try{
        tss_elem = document.getElementById('rbp-total-selected-p');
        ab_score_elem_arr = document.getElementById('ability-score-val-tbl').getElementsByTagName('input');

        for(var i = 0; i < ab_score_elem_arr.length; i++){
            if(!isNaN(ab_score_elem_arr[i].value))
                score += parseInt(ab_score_elem_arr[i].value);
        }

        tss_elem.innerText = score + "";   
    } catch(err){
        alert("An error occurred while updating total selected score");
    }
}

function update_total(){
    var score = 0;
    var tts_score;
    var total_elem;
    var rbp_hdn_val;

    rbp_hdn_val = document.getElementById('rbp-hdn').value;
    total_elem = document.getElementById('rbp-total-true-p');
    tts_score = document.getElementById('rbp-total-selected-p').innerText;
    
    try{
        if(!isNaN(parseInt(rbp_hdn_val)))
            score += parseInt(rbp_hdn_val);

        if(!isNaN(parseInt(tts_score)))
            score += parseInt(tts_score);

        total_elem.innerText = score + "";   
    } catch(err){
        alert("An error occurred while updating total score");
    }
}

function all_character_req_handler(){
    var table_body_elem;
    var json_all_char = {};
    
    if(this.readyState == '4'){
        if(this.status == '200'){
            json_all_char = JSON.parse(JSON.parse(this.responseText));

            if(json_all_char != null){
                table_body_elem = document.getElementById('character-list-body-container');
                table_body_elem.innerText = null;

                for(var json_char in json_all_char){
                    add_char_to_table(json_all_char[json_char]);
                }
            }

            prepare_table();   
        } else if(this.status == '500'){
            alert(this.responseText);
        }
    }
}

function escape_json_keywords(str){
    return str.replace(/\n/g, "\\n")
        .replace(/\'/g, "\\'")
        .replace(/\"/g, '\\"')
        .replace(/\&/g, "\\&")
        .replace(/\r/g, "\\r")
        .replace(/\t/g, "\\t")
        .replace(/\f/g, "\\f");
}

function decode_str(str){
    var e;
    var decoded_str;
    
    try{
        e = document.createElement('textarea');
        e.innerHTML = str;

        if(e.childNodes.length === 0)
            return "";

        decoded_str = e.childNodes[0].nodeValue;
        decoded_str = decoded_str.substr(1, decoded_str.length - 2);
        decoded_str = JSON.stringify(decoded_str);

        return decoded_str;   
    } catch(err){
        alert("An error occurred while handling information received from server");
    }
}