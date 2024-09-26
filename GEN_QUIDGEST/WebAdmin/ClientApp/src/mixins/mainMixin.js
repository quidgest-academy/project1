/** Reusable methods and data */
import moment from 'moment';
import { QUtils } from '@/utils/mainUtils';

export const reusableMixin = {
    methods: {
        navigateTo(event, name, hasSubmenu = false) { 
            var vm = this;
            vm.isSelected = false;
            vm.isMenuOpen = hasSubmenu ? !vm.isMenuOpen : false;

            vm.$router.push({ name: name, params: { culture: vm.currentLang, system: vm.currentYear } });

            if (vm.currentSelected) {
                vm.currentSelected.classList.remove('selected');
            }

            if (!hasSubmenu) {
                event.currentTarget.classList.add('selected');
                vm.currentSelected = event.currentTarget;
            } else {
                event.currentTarget.classList.add('selected');
                vm.currentSelected = event.currentTarget;
                if (vm.Model.Applications.length > 0) {
                    const firstSubItem = vm.Model.Applications[0];
                    vm.currentApp = firstSubItem.Id;
                }
            }
        },
        isEmptyArray(arr) {
            return !(arr && arr.length > 0);
        },
        isEmptyObject(obj) {
            return $.isEmptyObject(obj);
        },
        formatDate(date) {
            if ($.isEmptyObject(date)) return '';
            else if ($.type(date) === 'string') {
                date = QUtils.tryParseDate(date);
            }
            if ($.type(date) === 'date' || moment.isMoment(date)) {
                if (date._isAMomentObject) { date = date.toDate(); }
                var text = date.toLocaleDateString('pt-PT') + ' ' + date.toLocaleTimeString('pt-PT');
                if (text === 'Invalid Date Invalid Date' || date.getFullYear() <= 0) { // IE11 and Null date
                    text = '-';
                }
                return text;
            }
            else return '';
        }
    },
	data()
	{
		var vm = this;
		return {
			Resources: new Proxy({
					__v_skip: false,
					__v_isReactive: true,
					__v_isRef: false,
					__v_isReadonly: false,
					__v_raw: true
				}, {
					get(target, prop, receiver) {
						if (Reflect.has(target, prop))
							return Reflect.get(target, prop, receiver)
						return vm.$tm(prop)
					}
				})
			}
	}
};



function ParseQueryString(query) {
    let res = {};
    if (!query)
        return res;
    let vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        let pair = vars[i].split('=');
        let d0 = decodeURIComponent(pair[0]);
        let d1 = decodeURIComponent(pair[1]);
        res[d0] = d1;
    }
    return res;
}
function EncodeQueryString(obj) {
    //%26 : &
    //%3D : =
    //%25 : %
    //just replace these so the final config string is a bit more readable when encoding json
    let miniUriEncode = (s) => s.replaceAll('%', '%25').replaceAll('&', '%26').replaceAll('=', '%3D');
    return Object.entries(obj)
        .map(([key, value]) => miniUriEncode(key) + '=' + miniUriEncode(value))
        .join('&');
}
function NormalizeValue(option, config) {
    let res;
    if (option.Parent.length > 0 && config[option.Parent])
        res = config[option.Parent][option.PropertyName];
    else
        res = config[option.PropertyName];

    //json lists are going to be objects, so we need to transform them to strings
    if (option.Type.startsWith('List') && res)
        res = res.join(',');

    if (!res) res = "";
    return res;
}


export function ReadProviderConfig(Model, ProviderTypeList) {
    let tempConfig = [];
    if (!Model.Type)
        return tempConfig;

    let provider = ProviderTypeList.find(x => x.TypeFullName == Model.Type);
    if (!provider)
        return tempConfig;
    let c = Model.Config ? ParseQueryString(Model.Config) : {};

    //expand json string objects if they belong the the parent of an option
    for (const o of provider.Options) {
        if (o.Parent.length > 0 && (typeof c[o.Parent]) == 'string')
            c[o.Parent] = JSON.parse(c[o.Parent]);
    }

    //create the temporary value array that the UI will need to supply editors for
    tempConfig = provider.Options.map(o => ({
        Value: NormalizeValue(o, c),
        ...o
    }));

    return tempConfig;
}

export function WriteProviderConfig(tempConfig, Model, ProviderTypeList) {
    if (!Model.Type) return;

    let provider = ProviderTypeList.find(x => x.TypeFullName == Model.Type);
    if (!provider) return;
    let obj = {};

    //group all the options by parent
    for (const o of provider.Options) {
        let value = tempConfig.find(x => x.PropertyName == o.PropertyName).Value;
        if (!value || value == "")
            continue;

        //reconstitute json lists back into an object
        if (o.Type.startsWith('List'))
            value = value.split(',');

        if (o.Parent.length > 0) {
            if (!obj[o.Parent]) obj[o.Parent] = {};
            obj[o.Parent][o.PropertyName] = value;
        }
        else
            obj[o.PropertyName] = value;
    }

    //convert to json the parented options
    for (const o of provider.Options) {
        if (o.Parent.length > 0 && obj[o.Parent] && (typeof obj[o.Parent]) != 'string') {
            obj[o.Parent] = JSON.stringify(obj[o.Parent]);
        }
    }

    //query string encode all the values
    Model.Config = EncodeQueryString(obj);
}
