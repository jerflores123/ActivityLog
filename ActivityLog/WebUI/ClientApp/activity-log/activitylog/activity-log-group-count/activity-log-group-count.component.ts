import { Component, OnInit, Inject } from "@angular/core";
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialogConfig, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { AuthorizeService } from "src/api-authorization/authorize.service";
import { UiCommonService } from "src/app/ui-common/ui-common-service";
import { ActivityLogClient, GroupCountClient, PrivilegeClient, GetCurrentUserFeaturePrivilegeResponse, ActivityLogdto, ActivityLogGroupCountsdto, ActivityLogVm, UpdateGroupCountCommand } from "src/app/web-api-client";
//import { ActivityLogGroupCountDetailsComponent } from './activity-log-group-count-details/activity-log-group-count-details.component';
//import { UpdateGroupCountCommand }  from '../../../web-api-client'

export interface DialogData {
  groupCount: ActivityLogGroupCountsdto;

}

@Component({
  selector: 'app-activity-log-group-count',
  templateUrl: './activity-log-group-count.component.html',
  styleUrls: ['./activity-log-group-count.component.scss']
})
export class ActivityLogGroupCountComponent implements OnInit {

  groupCount: ActivityLogGroupCountsdto[] = [];
  vm: ActivityLogVm;
  Privilege: GetCurrentUserFeaturePrivilegeResponse;

  constructor(
    private groupCountClient: GroupCountClient,
    //private activityLogDelLimit: ActivityLogDelLimit,
    private privilegeClient: PrivilegeClient,
    private authorizeService: AuthorizeService,
    private uiService: UiCommonService,
    private ActivityLogClient: ActivityLogClient,
    public dialogRef: MatDialogRef<ActivityLogGroupCountComponent>,
    public dialog: MatDialog
  ) {

    this.privilegeClient.getCurrentUserFeaturePrivilege('Activity Log').subscribe(
      result => {
        this.Privilege = result;
        if (this.Privilege.read_priv)
          this.LoadGrid();

      },
      error => {
        console.error(error)
      }
    );

    this.getGroupCount();
    this.LoadGrid();
  }
  LoadGrid() {

    this.ActivityLogClient.getActivityLogGROUPCOUNTS().subscribe(
      result => {
        this.vm = result;
      }
    );
  }
  ngOnInit(): void {
  }

  edit(row: ActivityLogGroupCountsdto) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.id = "app-activity-log-group-count-details";
    dialogConfig.height = 'fit-content';
    dialogConfig.width = "300px";
    dialogConfig.data = {
      row: row,
      groupCount: this.groupCount

    };
    /*this.dialog.open(ActivityLogGroupCountDetailsComponent, dialogConfig).afterClosed().subscribe(res => {
      this.getGroupCount();
    });*/
  }

  getGroupCount() {

    this.ActivityLogClient.getActivityLogGROUPCOUNTS().subscribe(res => {
      this.groupCount = res.groupCount;
    });
  }
  submit(item: ActivityLogGroupCountsdto) {
    let alGroupCount = ActivityLogGroupCountsdto.fromJS({
      groupName: item.group_name,
      groupCount: item.group_counts
    });
    let row = { ...item }
    let postData = UpdateGroupCountCommand.fromJS({ activityLogGroupCountsDto: item });
    this.groupCountClient.update(postData).subscribe(res => {
      this.uiService.snackNotification('Selected Activity Log Group Count Record updated successfully!');
      this.dialogRef.close(true);
    });
  }
}
