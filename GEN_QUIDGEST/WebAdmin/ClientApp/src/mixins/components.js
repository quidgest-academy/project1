// All components
import text_input from '@/components/Text_input.vue';
import numeric_input from '@/components/Numeric_input.vue';
import checkbox_input from '@/components/Checkbox_input.vue';
import radio_input from '@/components/Radio_input.vue';
import select_input from '@/components/Select_input.vue';
import password_input from '@/components/Password_input.vue';
import static_text from '@/components/Static_text.vue';
import datetime_picker from '@/components/DateTime_input.vue';
import card_container from '@/components/Card_container.vue';
import row from '@/components/Row.vue';
import qtable from '@/components/Table/QTable.vue';
import progress_bar from '@/components/ProgressBar.vue';
import image_byte_array from '@/components/Image_byte_array.vue';
import image_input from '@/components/Image_input.vue';
import select_simple from '@/components/Select_simple.vue';
import textarea_input from '@/components/Textarea_input.vue';
import GroupBoxContainer from '@/components/GroupBoxContainer.vue';

// Quidgest UI
import {
    QButton,
	QButtonGroup,
	QIcon,
	QSelect,
	QTooltip
} from '@quidgest/ui/components'

export default function ComponentsInit(app) {
    // Inputs
    app.component('text-input', text_input);
    app.component('numeric-input', numeric_input);
    app.component('checkbox-input', checkbox_input);
    app.component('radio-input', radio_input);
    app.component('select-input', select_input);
    app.component('password-input', password_input);
    app.component('static-text', static_text);
    app.component('datetime-picker', datetime_picker);
    app.component('card', card_container);
    app.component('row', row);
    app.component('qtable', qtable);
    app.component('progress-bar', progress_bar);
    app.component('image-byte-array', image_byte_array);
    app.component('image-input', image_input);
    app.component('select-simple', select_simple);
    app.component('textarea-input', textarea_input);
    app.component('QGroupBoxContainer', GroupBoxContainer);

    app.component('QButton', QButton)
    app.component('QButtonGroup', QButtonGroup)
    app.component('QIcon', QIcon)
    app.component('QSelect', QSelect)
    app.component('QTooltip', QTooltip)
}
