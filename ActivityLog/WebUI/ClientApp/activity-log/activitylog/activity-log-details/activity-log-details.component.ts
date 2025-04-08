import { Component, OnInit, Inject } from "@angular/core";
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { Router } from '@angular/router';
import { timeStamp } from "console";
import { DATE } from "ngx-bootstrap/chronos/units/constants";
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { UiCommonService } from "src/app/ui-common/ui-common-service";
import { ActivityLogCboDatadto, ActivityLogClient, ActivityLogdto, StaffDto, ActivityLogVm, OffenderDto, CreateActivityLogCommand, UpdateActivityLogCommand } from "src/app/web-api-client";

export interface DialogData {
  sin: number;
  items: ActivityLogdto;
  eventData: ActivityLogCboDatadto[];
  areaData: ActivityLogCboDatadto[];
  groupData: ActivityLogCboDatadto[];
  update: boolean,
  staff: StaffDto[];
  offender: OffenderDto[];
  row: ActivityLogdto;
  row2: ActivityLogCboDatadto;
  Mythis: any;
}

@Component({
  selector: 'app-activity-log-details',
  templateUrl: './activity-log-details.component.html',
  styleUrls: ['./activity-log-details.component.scss']
})
export class ActivityLogDetailsComponent implements OnInit {
  vm: ActivityLogVm;
  methods: string[] = []
  methods2: string[] = []
  methods3: string[] = []
  perimeters: string[] = []
  staffList: string[] = [];
  offenderList: string[] = [];
  offenderList2: OffenderDto[];
  today = new Date();
  currentUser: any = "";
  optionDisabled: boolean = this.data.update;
  formGroup = new FormGroup({
    sin: new FormControl(),
    cbo_type: new FormControl('', Validators.required),
    cbo_type2: new FormControl('', Validators.required),
    cbo_type3: new FormControl('', Validators.required),
    cbo_type4: new FormControl('', Validators.required),
    staff_list: new FormControl('', Validators.required),
    method_value: new FormControl('', Validators.required),
    offender_list: new FormControl('', Validators.required),
    method_value3: new FormControl('', Validators.required),
    car_num: new FormControl('', Validators.required),
    cell_phone: new FormControl('', Validators.required),
    per_area: new FormControl('', Validators.required),
    comments: new FormControl(''),
  });



  constructor(
    private activityLogClient: ActivityLogClient,
    private authorizeService: AuthorizeService,
    private uiService: UiCommonService,
    private router: Router,
    public dialogRef: MatDialogRef<ActivityLogDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    this.LoadGrid(this.data.sin);
    this.methods = [...new Set(data.eventData.map(r => r.cbo_data))];
    this.methods2 = [...new Set(data.areaData.map(r => r.cbo_data))];
    this.methods3 = [...new Set(data.groupData.map(r => r.cbo_data))];
    this.staffList = [...new Set(data.staff.map(r => r.last_name))];
    this.offenderList = [...new Set(data.offender.map(r => r.last_name))];
    this.formGroup.get('sin').setValue(this.data.sin);
    this.perimeters = ["Perimeter", "Zone 1", "Zone 2", "Zone 3", "Zone 4", "OBC"];
    if (data.update) {
      this.formGroup.get('cbo_type').setValue(data.row.event_type);
    }
    this.formGroup.updateValueAndValidity();
  }
  LoadGrid(sin) {
    this.activityLogClient.getAll(sin).subscribe(
      result => {
        this.vm = result;
      },
      error => console.error(error)
    );
  }
  ngOnInit(): void {
    this.authorizeService.currentUser.subscribe(res => {
      this.currentUser = res?.userId;
    });

    this.formGroup.controls.method_value.valueChanges.subscribe(value => {
      value !== '1' ? this.formGroup.controls.offender_list.disable() : this.formGroup.controls.offender_list.enable();
    });
  }

  onMethodSelection() {
    let selected_method = this.formGroup.get('cbo_type').value;
    /*this.staffList = this.data.staff.filter(r => r. === selected_method).map(r => r.first_name);
   this.formGroup.get('staff_list').setValue(this.staffList[0]);
   if (selected_method === "Attendance")
     this.formGroup.get('method_value2').setValidators(Validators.required);
   this.formGroup.updateValueAndValidity();*/
  }

  getLabel() {
    let type = this.formGroup.get('cbo_type').value;
    if (type === "Apprehension") {
      return "Apprehension Location"
    } else if (type === "Escape") {
      return "From Location";
    } else if (type === "Movements") {
      return "Count";
    } else if (type === "Attendance") {
      return "In/Out";
    } else if (type === "Transport") {
      return "Cell Phone";
    }
    return type;
  }

  //certain events with 2 input fields
  getStaff() {
    let type = this.formGroup.get('cbo_type').value;
    if (type === 'Attendance' || type === "Perimeter Search" || type === "Search") {
      return "Staff Name";
    }
    return type;
  }

  getFormValue(controlName: string) {
    return this.formGroup.get(controlName).value;
  }

  onGroupSelect(i) {

    this.vm.activityLog;
    this.vm.allOffenders;
    this.vm.aztecs;
    this.vm.staff;
    //this.offenderList = this.vm.find(item => item.country === selectedCountry).stateList;
    if (i === 'AZTECS') {
      this.offenderList2 = this.vm.aztecs;
    } else if (i === 'CHOICES') {
      this.offenderList2 = this.vm.choices;
    } else if (i === 'ELEMENTS') {
      this.offenderList2 = this.vm.elements
    } else if (i === 'EVEREST') {
      this.offenderList2 = this.vm.everest
    } else if (i === 'INCAS') {
      this.offenderList2 = this.vm.incas
    } else if (i === 'MAYAS') {
      this.offenderList2 = this.vm.mayas
    } else if (i === 'O/A') {
      this.offenderList2 = this.vm.oa
    } else if (i === 'PATHWAYS') {
      this.offenderList2 = this.vm.pathways
    } else if (i === 'SAWTOOTH') {
      this.offenderList2 = this.vm.sawtooth
    } else if (i === 'SOLUTIONS') {
      this.offenderList2 = this.vm.solutions
    } else if (i === 'STAGING') {
      this.offenderList2 = this.vm.staging
    } else if (i === 'VANGAURD') {
      this.offenderList2 = this.vm.vanguard
    } else if (i === 'WASATCH') {
      this.offenderList2 = this.vm.wasatch
    }
  }

  submit() {
    this.data.items.description = '';
    var per_area_string = '';
    per_area_string = this.formGroup.get('per_area').value;
    var newstr = per_area_string.toString().replace(/,/g, " ");

    this.formGroup.patchValue({
      per_area: [newstr],
    });

    var str = '';
    Object.keys(this.formGroup.controls).forEach(key => {
      this.formGroup.get(key).markAsDirty();
      if (this.formGroup.get(key).value != "" || this.formGroup.get(key).value != null) {
        str += this.formGroup.get(key).value + ", ";
      }
    });

    let activityLogDto2 = ActivityLogdto.fromJS({
      description: str,
      comments: this.getFormValue("comments"),
      event_type: this.getFormValue("cbo_type"),
      is_active: 1,
      created_by: this.currentUser,
    });

    let activityLogDto3 = ActivityLogdto.fromJS({
      description: str,
      comments: this.getFormValue("comments"),
      event_type: this.getFormValue("cbo_type"),
      is_active: 1,
      created_by: this.currentUser,
    });

    let activityLogVm = ActivityLogVm.fromJS({
      activityLog: [activityLogDto2]
    });

    if (this.data.update) {
      let row = { ...this.data.row, ...this.formGroup.value };
      row.description = str;
      let postData = UpdateActivityLogCommand.fromJS({ activityLogDto: row });
      this.activityLogClient.update(postData).subscribe(res => {
        this.uiService.snackNotification('Selected Activity Log Record updated successfully!');
        this.dialogRef.close(true);
        this.data.Mythis.FillGridActivtyLog();
      });
    } else {
      let postData = CreateActivityLogCommand.fromJS({ activityLogDto: activityLogDto2 });
      this.activityLogClient.create(postData).subscribe(res => {
        this.uiService.snackNotification('Activity Log created successfully!');
        this.dialogRef.close(true);
        this.data.Mythis.FillGridActivtyLog();
      });
    }
  }
}
