import { FormGroup } from '@angular/forms';

export function transferToFormData(form: FormGroup, fileControls: Array<string> = []): FormData {
    const formData = new FormData();
    const keys = Object.keys(form.controls);
    keys.forEach(k => {
        const val: any = form.controls[k].value;
        if (fileControls.some(f => f === k)) {
            const fs = val as Array<File>;
            if (fs?.length) {
                for (let i = 0; i < fs.length; i++) {
                    formData.append(k, fs[i], fs[i]['name']);
                }
            }
        } else {
            formData.append(k, val);
        }
    });
    return formData;
}
