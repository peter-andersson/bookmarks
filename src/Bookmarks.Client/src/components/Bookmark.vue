<script setup lang="ts">
import type { BookmarkModel } from "@/models";

defineProps<{
  bookmark: BookmarkModel,
  progress: boolean,
  buttonText: string
}>();

const emit = defineEmits<{
  (e: "ok"): void
  (e: "cancel"): void
}>();

// TODO: Tags
// TODO: Autocomplete on tag names?
// TODO: Load info from url

function ok() {
  emit("ok")
}

function cancel() {
  emit("cancel");
}
</script>

<template>
  <div class="mb-3">
    <label for="url" class="form-label">URL</label>
    <input type="text" class="form-control" id="url" v-model="bookmark.url">
  </div>
  <div class="mb-3">
    <label for="tags" class="form-label">Tags</label>
    <input type="text" class="form-control" id="tags" v-model="bookmark.tags">
    <div class="form-text">Enter tags separated by space.</div>
  </div>
  <div class="mb-3">
    <label for="title" class="form-label">Title</label>
    <input type="text" class="form-control" id="title" v-model="bookmark.title">
    <div class="form-text">Optional, load from website.</div>
  </div>
  <div class="mb-3">
    <label for="description" class="form-label">Description</label>
    <input type="text" class="form-control" id="description" v-model="bookmark.description">
    <div class="form-text">Optional, load from website.</div>
  </div>

  <button type="button" class="btn btn-success" @click="ok" v-bind:disabled="progress">
    <span v-if="progress" class="spinner-border spinner-border-sm" aria-hidden="true"></span>
    <span v-if="!progress">{{ buttonText }}</span>
  </button>
  <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
</template>

<style scoped>

</style>