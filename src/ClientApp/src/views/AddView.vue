<script setup lang="ts">
  import router from '@/router';

  let url = '';
  let tags = '';
  let title = '';
  let description = '';

  function cancel() {
    router.push('/');
  }

  function add() {
    // TODO: Send to api
    alert('Add');
  }
</script>

<template>
  <h1>Add bookmark</h1>

    <div class="mb-3">
      <label for="url" class="form-label">URL</label>
      <input type="text" class="form-control" id="url" v-model="url">
    </div>
  <div class="mb-3">
    <label for="tags" class="form-label">Tags</label>
    <input type="text" class="form-control" id="tags" v-model="tags">
    <div class="form-text">Enter tags separated by space.</div>
  </div>
  <div class="mb-3">
    <label for="title" class="form-label">Title</label>
    <input type="text" class="form-control" id="title" v-model="title">
    <div class="form-text">Optional, load from website.</div>
  </div>
  <div class="mb-3">
    <label for="description" class="form-label">Description</label>
    <input type="text" class="form-control" id="description" v-model="description">
    <div class="form-text">Optional, load from website.</div>
  </div>

  <button type="button" class="btn btn-success" @click="add">Add</button>
  <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
</template>
