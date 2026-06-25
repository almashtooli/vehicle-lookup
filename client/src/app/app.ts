import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Make, Model, VehicleService, VehicleType } from './vehicle.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private vehicles = inject(VehicleService);

  makes = signal<Make[]>([]);
  makeFilter = signal('');
  selectedMake = signal<Make | null>(null);

  years = Array.from({ length: new Date().getFullYear() + 1 - 1995 + 1 }, (_, i) => new Date().getFullYear() + 1 - i);
  selectedYear = signal<number | null>(null);

  vehicleTypes = signal<VehicleType[]>([]);
  selectedType = signal<string>('');

  models = signal<Model[]>([]);
  loadingModels = signal(false);
  searched = signal(false);

  filteredMakes = computed(() => {
    const f = this.makeFilter().trim().toLowerCase();
    if (!f) return [];
    return this.makes().filter(m => m.make_Name.toLowerCase().includes(f)).slice(0, 50);
  });

  constructor() {
    this.vehicles.getMakes().subscribe(m => this.makes.set(m));
  }

  pickMake(m: Make) {
    this.selectedMake.set(m);
    this.makeFilter.set(m.make_Name);
    this.vehicleTypes.set([]);
    this.selectedType.set('');
    this.models.set([]);
    this.searched.set(false);
    this.vehicles.getVehicleTypes(m.make_ID).subscribe(t => this.vehicleTypes.set(t));
  }

  loadModels() {
    const make = this.selectedMake();
    const year = this.selectedYear();
    if (!make || !year) return;
    this.loadingModels.set(true);
    this.searched.set(true);
    this.vehicles.getModels(make.make_ID, year, this.selectedType() || undefined)
      .subscribe({
        next: m => { this.models.set(m); this.loadingModels.set(false); },
        error: () => { this.models.set([]); this.loadingModels.set(false); }
      });
  }
}